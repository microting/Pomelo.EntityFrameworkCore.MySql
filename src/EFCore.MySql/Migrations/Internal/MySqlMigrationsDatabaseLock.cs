// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

#if EFCORE10_OR_GREATER

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pomelo.EntityFrameworkCore.MySql.Migrations.Internal
{
    /// <summary>
    /// MySQL-specific implementation of IMigrationsDatabaseLock for EF Core 10+.
    /// This provides database-level locking during migrations to prevent concurrent migration execution.
    /// </summary>
    public class MySqlMigrationsDatabaseLock : IMigrationsDatabaseLock
    {
        private readonly string _connectionString;
        private readonly string _lockName;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the MySqlMigrationsDatabaseLock class.
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="lockName">The name of the lock (optional, defaults to migration lock)</param>
        public MySqlMigrationsDatabaseLock(string connectionString, string lockName = "EFCore_Migration_Lock")
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _lockName = lockName ?? throw new ArgumentNullException(nameof(lockName));
        }

        /// <summary>
        /// Releases the database lock.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                ReleaseLock();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Asynchronously releases the database lock.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await ReleaseLockAsync();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        private void ReleaseLock()
        {
            // Implementation would use MySQL's GET_LOCK/RELEASE_LOCK functions
            // This is a placeholder for the actual implementation when EF Core 10 is available
            
            // Example MySQL lock release:
            // using var connection = new MySqlConnection(_connectionString);
            // connection.Open();
            // using var command = connection.CreateCommand();
            // command.CommandText = $"SELECT RELEASE_LOCK('{_lockName}')";
            // command.ExecuteScalar();
        }

        private async Task ReleaseLockAsync()
        {
            // Implementation would use MySQL's GET_LOCK/RELEASE_LOCK functions
            // This is a placeholder for the actual implementation when EF Core 10 is available
            
            // Example MySQL async lock release:
            // using var connection = new MySqlConnection(_connectionString);
            // await connection.OpenAsync();
            // using var command = connection.CreateCommand();
            // command.CommandText = $"SELECT RELEASE_LOCK('{_lockName}')";
            // await command.ExecuteScalarAsync();
            
            await Task.CompletedTask; // Placeholder
        }
    }
}

#endif