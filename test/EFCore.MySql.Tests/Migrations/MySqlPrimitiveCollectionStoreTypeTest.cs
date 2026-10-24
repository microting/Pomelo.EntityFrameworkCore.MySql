using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.Migrations
{
    // Database independent: only builds the model, no connection is opened.
    public class MySqlPrimitiveCollectionStoreTypeTest
    {
        public class Account
        {
            public int Id { get; set; }
            public string[] Roles { get; set; }
            public List<int> Numbers { get; set; }
            public string[] ExplicitLongText { get; set; }
        }

        public class TestContext : DbContext
        {
            private readonly string _serverVersion;
            private readonly bool _jsonColumns;

            public TestContext(string serverVersion, bool jsonColumns)
            {
                _serverVersion = serverVersion;
                _jsonColumns = jsonColumns;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseMySql(
                    "Server=localhost;Database=x",
                    ServerVersion.Parse(_serverVersion),
                    b => b.EnableJsonColumnsForPrimitiveCollections(_jsonColumns));

            protected override void OnModelCreating(ModelBuilder modelBuilder)
                => modelBuilder.Entity<Account>().Property(e => e.ExplicitLongText).HasColumnType("longtext");
        }

        private static Dictionary<string, string> GetStoreTypes(string serverVersion, bool jsonColumns)
        {
            using var context = new TestContext(serverVersion, jsonColumns);
            return context.GetService<IDesignTimeModel>().Model
                .GetRelationalModel().Tables.Single().Columns.ToDictionary(c => c.Name, c => c.StoreType);
        }

        [Theory]
        [InlineData("8.0.30-mysql")]
        [InlineData("5.7.8-mysql")]
        [InlineData("10.6.0-mariadb")]
        [InlineData("10.2.4-mariadb")]
        public void Opt_in_maps_primitive_collections_to_json(string serverVersion)
        {
            var columns = GetStoreTypes(serverVersion, true);

            Assert.Equal("json", columns[nameof(Account.Roles)]);
            Assert.Equal("json", columns[nameof(Account.Numbers)]);
            Assert.Equal("longtext", columns[nameof(Account.ExplicitLongText)]);
        }

        [Theory]
        [InlineData("8.0.30-mysql")]
        [InlineData("10.6.0-mariadb")]
        public void Default_keeps_longtext_for_primitive_collections(string serverVersion)
        {
            var columns = GetStoreTypes(serverVersion, false);

            Assert.Equal("longtext", columns[nameof(Account.Roles)]);
            Assert.Equal("longtext", columns[nameof(Account.Numbers)]);
        }

        [Theory]
        [InlineData("5.6.0-mysql")]
        [InlineData("10.1.0-mariadb")]
        public void Opt_in_has_no_effect_when_server_lacks_json_support(string serverVersion)
        {
            var columns = GetStoreTypes(serverVersion, true);

            Assert.Equal("longtext", columns[nameof(Account.Roles)]);
            Assert.Equal("longtext", columns[nameof(Account.Numbers)]);
        }
    }
}
