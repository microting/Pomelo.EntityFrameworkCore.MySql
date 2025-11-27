// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    public class MySqlJsonTypeMapping<T> : MySqlJsonTypeMapping
    {
        public static new MySqlJsonTypeMapping<T> Default { get; } = new("json", null, null, false, true);

        public MySqlJsonTypeMapping(
            [NotNull] string storeType,
            [CanBeNull] ValueConverter valueConverter,
            [CanBeNull] ValueComparer valueComparer,
            bool noBackslashEscapes,
            bool replaceLineBreaksWithCharFunction)
            : base(
                storeType,
                typeof(T),
                valueConverter,
                valueComparer,
                noBackslashEscapes,
                replaceLineBreaksWithCharFunction)
        {
        }

        protected MySqlJsonTypeMapping(
            RelationalTypeMappingParameters parameters,
            MySqlDbType mySqlDbType,
            bool noBackslashEscapes,
            bool replaceLineBreaksWithCharFunction)
            : base(parameters, mySqlDbType, noBackslashEscapes, replaceLineBreaksWithCharFunction)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlJsonTypeMapping<T>(parameters, MySqlDbType, NoBackslashEscapes, ReplaceLineBreaksWithCharFunction);

        protected override RelationalTypeMapping Clone(bool? noBackslashEscapes = null, bool? replaceLineBreaksWithCharFunction = null)
            => new MySqlJsonTypeMapping<T>(
                Parameters,
                MySqlDbType,
                noBackslashEscapes ?? NoBackslashEscapes,
                replaceLineBreaksWithCharFunction ?? ReplaceLineBreaksWithCharFunction);
    }

    public abstract class MySqlJsonTypeMapping : MySqlStringTypeMapping, IMySqlCSharpRuntimeAnnotationTypeMappingCodeGenerator
    {
        private static readonly MethodInfo _getString
            = typeof(DbDataReader).GetRuntimeMethod(nameof(DbDataReader.GetString), new[] { typeof(int) });

        // Cache reflection lookups for performance
        private static readonly PropertyInfo _utf8Property
            = typeof(System.Text.Encoding).GetProperty(nameof(System.Text.Encoding.UTF8));
        private static readonly MethodInfo _getBytesMethod
            = typeof(System.Text.Encoding).GetMethod(nameof(System.Text.Encoding.GetBytes), new[] { typeof(string) });
        private static readonly ConstructorInfo _memoryStreamCtor
            = typeof(System.IO.MemoryStream).GetConstructor(new[] { typeof(byte[]) });

        public MySqlJsonTypeMapping(
            [NotNull] string storeType,
            [NotNull] Type clrType,
            [CanBeNull] ValueConverter valueConverter,
            [CanBeNull] ValueComparer valueComparer,
            bool noBackslashEscapes,
            bool replaceLineBreaksWithCharFunction)
            : base(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(
                        clrType,
                        valueConverter,
                        valueComparer),
                    storeType,
                    unicode: true),
                MySqlDbType.JSON,
                noBackslashEscapes,
                replaceLineBreaksWithCharFunction,
                false,
                false)
        {
            if (storeType != "json")
            {
                throw new ArgumentException($"The store type '{nameof(storeType)}' must be 'json'.", nameof(storeType));
            }
        }

        protected MySqlJsonTypeMapping(
            RelationalTypeMappingParameters parameters,
            MySqlDbType mySqlDbType,
            bool noBackslashEscapes,
            bool replaceLineBreaksWithCharFunction)
            : base(
                parameters,
                mySqlDbType,
                noBackslashEscapes,
                replaceLineBreaksWithCharFunction,
                isUnquoted: false,
                forceToString: false)
        {
        }

        /// <summary>
        /// Supports compiled models via IMySqlCSharpRuntimeAnnotationTypeMappingCodeGenerator.Create.
        /// </summary>
        protected abstract RelationalTypeMapping Clone(
            bool? noBackslashEscapes = null,
            bool? replaceLineBreaksWithCharFunction = null);

        protected override void ConfigureParameter(DbParameter parameter)
        {
            base.ConfigureParameter(parameter);

            // MySqlConnector does not know how to handle our custom MySqlJsonString type, that could be used when a
            // string parameter is explicitly cast to it.
            if (parameter.Value is MySqlJsonString mySqlJsonString)
            {
                parameter.Value = (string)mySqlJsonString;
            }
        }

        /// <summary>
        /// Returns the method to be used for reading JSON values from the database.
        /// MySQL stores JSON as strings, so we use GetString instead of the default GetFieldValue&lt;T&gt;.
        /// This prevents EF Core from trying to convert from string to MemoryStream for complex JSON types.
        /// </summary>
        public override MethodInfo GetDataReaderMethod()
            => _getString;

        /// <summary>
        /// Customizes the data reader expression for JSON types.
        /// MySQL stores JSON as strings, but EF Core expects MemoryStream for complex JSON types.
        /// For regular JSON columns mapped to string CLR type, no conversion is needed.
        /// This method creates an expression that converts the string to a MemoryStream containing UTF-8 encoded bytes
        /// only when the CLR type is not string (i.e., for complex types).
        /// </summary>
        public override Expression CustomizeDataReaderExpression(Expression expression)
        {
            // Only convert to MemoryStream for non-string CLR types (complex JSON types).
            // For string CLR types (regular JSON columns), return the string as-is.
            if (expression.Type == typeof(string) && ClrType != typeof(string))
            {
                // Validate that reflection lookups succeeded
                if (_utf8Property == null || _getBytesMethod == null || _memoryStreamCtor == null)
                {
                    throw new InvalidOperationException(
                        "Failed to find required reflection members for JSON type mapping. " +
                        "This may indicate an incompatible version of the .NET runtime.");
                }

                // Convert string to MemoryStream: new MemoryStream(Encoding.UTF8.GetBytes(stringValue))
                // This is needed for complex JSON types where EF Core expects a stream
                return Expression.New(
                    _memoryStreamCtor,
                    Expression.Call(
                        Expression.Property(null, _utf8Property),
                        _getBytesMethod,
                        expression));
            }

            return base.CustomizeDataReaderExpression(expression);
        }

        void IMySqlCSharpRuntimeAnnotationTypeMappingCodeGenerator.Create(
            CSharpRuntimeAnnotationCodeGeneratorParameters codeGeneratorParameters,
            CSharpRuntimeAnnotationCodeGeneratorDependencies codeGeneratorDependencies)
        {
            var defaultTypeMapping = Default;
            if (defaultTypeMapping == this)
            {
                return;
            }

            var code = codeGeneratorDependencies.CSharpHelper;

            var cloneParameters = new List<string>();

            if (NoBackslashEscapes != defaultTypeMapping.NoBackslashEscapes)
            {
                cloneParameters.Add($"noBackslashEscapes: {code.Literal(NoBackslashEscapes)}");
            }

            if (ReplaceLineBreaksWithCharFunction != defaultTypeMapping.ReplaceLineBreaksWithCharFunction)
            {
                cloneParameters.Add($"replaceLineBreaksWithCharFunction: {code.Literal(ReplaceLineBreaksWithCharFunction)}");
            }

            if (cloneParameters.Any())
            {
                var mainBuilder = codeGeneratorParameters.MainBuilder;

                mainBuilder.AppendLine(";");

                mainBuilder
                    .AppendLine($"{codeGeneratorParameters.TargetName}.TypeMapping = (({code.Reference(GetType())}){codeGeneratorParameters.TargetName}.TypeMapping).Clone(")
                    .IncrementIndent();

                for (var i = 0; i < cloneParameters.Count; i++)
                {
                    if (i > 0)
                    {
                        mainBuilder.AppendLine(",");
                    }

                    mainBuilder.Append(cloneParameters[i]);
                }

                mainBuilder
                    .Append(")")
                    .DecrementIndent();
            }
        }
    }
}
