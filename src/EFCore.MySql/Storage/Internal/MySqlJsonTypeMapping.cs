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
    /// <summary>
    /// Type mapping for complex JSON types (used when JSON is mapped to complex .NET types).
    /// Always converts string to MemoryStream since EF Core expects MemoryStream for complex JSON.
    /// </summary>
    public class MySqlComplexJsonTypeMapping : MySqlJsonTypeMapping<string>
    {
        public MySqlComplexJsonTypeMapping(
            [NotNull] string storeType,
            [CanBeNull] ValueConverter valueConverter,
            [CanBeNull] ValueComparer valueComparer,
            bool noBackslashEscapes,
            bool replaceLineBreaksWithCharFunction)
            : base(storeType, valueConverter, valueComparer, noBackslashEscapes, replaceLineBreaksWithCharFunction)
        {
            Console.WriteLine($"[DEBUG] MySqlComplexJsonTypeMapping constructor called - StoreType: {storeType}, Converter: {valueConverter?.GetType().Name ?? "null"}");
        }

        protected MySqlComplexJsonTypeMapping(
            RelationalTypeMappingParameters parameters,
            MySqlDbType mySqlDbType,
            bool noBackslashEscapes,
            bool replaceLineBreaksWithCharFunction)
            : base(parameters, mySqlDbType, noBackslashEscapes, replaceLineBreaksWithCharFunction)
        {
            Console.WriteLine($"[DEBUG] MySqlComplexJsonTypeMapping protected constructor called - ClrType: {ClrType?.Name ?? "null"}, StoreType: {StoreType}");
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        {
            Console.WriteLine($"[DEBUG] MySqlComplexJsonTypeMapping.Clone(parameters) called - ClrType: {parameters.CoreParameters.ClrType?.Name ?? "null"}");
            return new MySqlComplexJsonTypeMapping(parameters, MySqlDbType, NoBackslashEscapes, ReplaceLineBreaksWithCharFunction);
        }

        protected override RelationalTypeMapping Clone(bool? noBackslashEscapes = null, bool? replaceLineBreaksWithCharFunction = null)
        {
            Console.WriteLine($"[DEBUG] MySqlComplexJsonTypeMapping.Clone(bool) called - ClrType: {ClrType?.Name ?? "null"}, StoreType: {StoreType}");
            return new MySqlComplexJsonTypeMapping(
                Parameters,
                MySqlDbType,
                noBackslashEscapes ?? NoBackslashEscapes,
                replaceLineBreaksWithCharFunction ?? ReplaceLineBreaksWithCharFunction);
        }

        /// <summary>
        /// Returns the method to be used for reading JSON values from the database.
        /// MySQL stores JSON as strings, so we use GetString.
        /// </summary>
        public override MethodInfo GetDataReaderMethod()
        {
            Console.WriteLine($"[DEBUG] MySqlComplexJsonTypeMapping.GetDataReaderMethod() called - ClrType: {ClrType?.Name ?? "null"}, returning: {_getString?.Name ?? "null"}");
            return _getString;
        }

        /// <summary>
        /// For complex JSON, we ALWAYS convert string to MemoryStream.
        /// </summary>
        public override Expression CustomizeDataReaderExpression(Expression expression)
        {
            Console.WriteLine($"[DEBUG] MySqlComplexJsonTypeMapping.CustomizeDataReaderExpression() called - ExpressionType: {expression.Type?.Name ?? "null"}, ClrType: {ClrType?.Name ?? "null"}");
            
            if (expression.Type == typeof(string))
            {
                // Validate that reflection lookups succeeded (using cached members from base class)
                if (_utf8Property == null || _getBytesMethod == null || _memoryStreamCtor == null)
                {
                    throw new InvalidOperationException(
                        "Failed to find required reflection members for JSON type mapping.");
                }

                Console.WriteLine($"[DEBUG] MySqlComplexJsonTypeMapping: Converting string expression to MemoryStream");
                
                // Convert string to MemoryStream: new MemoryStream(Encoding.UTF8.GetBytes(stringValue))
                return Expression.New(
                    _memoryStreamCtor,
                    Expression.Call(
                        Expression.Property(null, _utf8Property),
                        _getBytesMethod,
                        expression));
            }

            Console.WriteLine($"[DEBUG] MySqlComplexJsonTypeMapping: No conversion, calling base.CustomizeDataReaderExpression");
            return base.CustomizeDataReaderExpression(expression);
        }

        public override string GenerateSqlLiteral(object value)
        {
            Console.WriteLine($"[DEBUG] MySqlComplexJsonTypeMapping.GenerateSqlLiteral called - value type: {value?.GetType()?.Name ?? "null"}");
            var result = base.GenerateSqlLiteral(value);
            Console.WriteLine($"[DEBUG] MySqlComplexJsonTypeMapping.GenerateSqlLiteral result: {result}");
            return result;
        }

        public override string GenerateProviderValueSqlLiteral(object value)
        {
            Console.WriteLine($"[DEBUG] MySqlComplexJsonTypeMapping.GenerateProviderValueSqlLiteral called - value type: {value?.GetType()?.Name ?? "null"}");
            var result = base.GenerateProviderValueSqlLiteral(value);
            Console.WriteLine($"[DEBUG] MySqlComplexJsonTypeMapping.GenerateProviderValueSqlLiteral result: {result}");
            return result;
        }
    }

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
        protected static readonly MethodInfo _getString
            = typeof(DbDataReader).GetRuntimeMethod(nameof(DbDataReader.GetString), new[] { typeof(int) });

        // Cache reflection lookups for performance
        protected static readonly PropertyInfo _utf8Property
            = typeof(System.Text.Encoding).GetProperty(nameof(System.Text.Encoding.UTF8));
        protected static readonly MethodInfo _getBytesMethod
            = typeof(System.Text.Encoding).GetMethod(nameof(System.Text.Encoding.GetBytes), new[] { typeof(string) });
        protected static readonly ConstructorInfo _memoryStreamCtor
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
