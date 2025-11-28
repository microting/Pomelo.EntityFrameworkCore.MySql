// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.Internal
{
    /// <summary>
    /// Type mapping for complex JSON types (types mapped with .ToJson()).
    /// This mapping handles the conversion from MySQL's string-based JSON to MemoryStream expected by EF Core.
    /// </summary>
    public class MySqlStructuralJsonTypeMapping : JsonTypeMapping
    {
        private static readonly MethodInfo _createUtf8StreamMethod
            = typeof(MySqlStructuralJsonTypeMapping).GetMethod(nameof(CreateUtf8Stream), new[] { typeof(string) });

        private static readonly MethodInfo _getStringMethod
            = typeof(DbDataReader).GetRuntimeMethod(nameof(DbDataReader.GetString), new[] { typeof(int) });

        public static MySqlStructuralJsonTypeMapping Default { get; } = new("json");

        public MySqlStructuralJsonTypeMapping(string storeType)
            : base(storeType, typeof(JsonTypePlaceholder), System.Data.DbType.String)
        {
            Console.WriteLine($"[DEBUG] MySqlStructuralJsonTypeMapping created - StoreType: {storeType}, ClrType: JsonTypePlaceholder");
        }

        protected MySqlStructuralJsonTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
            Console.WriteLine($"[DEBUG] MySqlStructuralJsonTypeMapping cloned - StoreType: {parameters.StoreType}");
        }

        /// <summary>
        /// MySQL stores JSON as strings, so we read using GetString.
        /// </summary>
        public override MethodInfo GetDataReaderMethod()
        {
            Console.WriteLine("[DEBUG] MySqlStructuralJsonTypeMapping.GetDataReaderMethod() called - returning DbDataReader.GetString");
            return _getStringMethod;
        }

        /// <summary>
        /// Converts the string read from MySQL to a MemoryStream for EF Core's JSON processing.
        /// </summary>
        public static MemoryStream CreateUtf8Stream(string json)
            => new MemoryStream(Encoding.UTF8.GetBytes(json ?? string.Empty));

        /// <summary>
        /// Customizes the data reader expression to convert string to MemoryStream.
        /// </summary>
        public override Expression CustomizeDataReaderExpression(Expression expression)
        {
            Console.WriteLine("[DEBUG] MySqlStructuralJsonTypeMapping.CustomizeDataReaderExpression() called - converting string to MemoryStream");
            return Expression.Call(_createUtf8StreamMethod, expression);
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new MySqlStructuralJsonTypeMapping(parameters);

        protected string EscapeSqlLiteral(string literal)
            => literal.Replace("'", "''");

        protected override string GenerateNonNullSqlLiteral(object value)
            => $"'{EscapeSqlLiteral((string)value)}'";

        protected override void ConfigureParameter(DbParameter parameter)
        {
            // MySQL uses JSON db type for JSON columns
            if (parameter is MySqlParameter mySqlParameter)
            {
                mySqlParameter.MySqlDbType = MySqlDbType.JSON;
            }

            base.ConfigureParameter(parameter);
        }
    }
}
