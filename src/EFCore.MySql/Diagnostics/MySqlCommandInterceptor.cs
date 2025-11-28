// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Pomelo.EntityFrameworkCore.MySql.Diagnostics
{
    /// <summary>
    /// Debug interceptor to log SQL commands
    /// </summary>
    public class MySqlCommandInterceptor : DbCommandInterceptor
    {
        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            Console.WriteLine($"[DEBUG SQL] Executing command:");
            Console.WriteLine($"[DEBUG SQL] {command.CommandText}");
            Console.WriteLine($"[DEBUG SQL] Parameters: {command.Parameters.Count}");
            return base.ReaderExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[DEBUG SQL] Executing command async:");
            Console.WriteLine($"[DEBUG SQL] {command.CommandText}");
            Console.WriteLine($"[DEBUG SQL] Parameters: {command.Parameters.Count}");
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> NonQueryExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<int> result)
        {
            Console.WriteLine($"[DEBUG SQL NonQuery] Executing command:");
            Console.WriteLine($"[DEBUG SQL NonQuery] {command.CommandText}");
            return base.NonQueryExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[DEBUG SQL NonQuery] Executing command async:");
            Console.WriteLine($"[DEBUG SQL NonQuery] {command.CommandText}");
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }
    }
}
