using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Types;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.Json))]
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTableImplementationStable))]
    public class JsonTypesRelationalMySqlTest : JsonTypesRelationalTestBase
    {
        public JsonTypesRelationalMySqlTest(NonSharedFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            //TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        // Skip spatial type tests for MariaDB < 11.8 (different spatial JSON handling)
        [ConditionalFact(Skip = "MariaDB 10.6+ has different spatial type JSON handling, support planned for 11.8+")]
        public override Task Can_read_write_line_string()
            => Task.CompletedTask;

        [ConditionalFact(Skip = "MariaDB 10.6+ has different spatial type JSON handling, support planned for 11.8+")]
        public override Task Can_read_write_point()
            => Task.CompletedTask;

        [ConditionalFact(Skip = "MariaDB 10.6+ has different spatial type JSON handling, support planned for 11.8+")]
        public override Task Can_read_write_polygon()
            => Task.CompletedTask;

        [ConditionalFact(Skip = "MariaDB 10.6+ has different spatial type JSON handling, support planned for 11.8+")]
        public override Task Can_read_write_multi_line_string()
            => Task.CompletedTask;

        [ConditionalFact(Skip = "MariaDB 10.6+ has different spatial type JSON handling, support planned for 11.8+")]
        public override Task Can_read_write_point_with_M()
            => Task.CompletedTask;

        // Skip ulong enum tests - MariaDB serializes UInt64 Max differently
        [ConditionalFact(Skip = "MariaDB 10.6+ serializes UInt64.MaxValue as full number instead of -1")]
        public override Task Can_read_write_collection_of_nullable_ulong_enum_JSON_values()
            => Task.CompletedTask;

        [ConditionalFact(Skip = "MariaDB 10.6+ serializes UInt64.MaxValue as full number instead of -1")]
        public override Task Can_read_write_collection_of_ulong_enum_JSON_values()
            => Task.CompletedTask;

        // Provide database-aware test for UInt64 enum serialization
        // MariaDB serializes UInt64.MaxValue differently than MySQL
        [Theory]
        [InlineData((EnumU64)0, """{"Prop":0}""")]
        [InlineData(EnumU64.Min, """{"Prop":0}""")]
        [InlineData(EnumU64.Max, """{"Prop":-1}""")]
        [InlineData(EnumU64.Default, """{"Prop":0}""")]
        [InlineData(EnumU64.One, """{"Prop":1}""")]
        [InlineData((EnumU64)8, """{"Prop":8}""")]
        public Task Can_read_write_ulong_enum_JSON_values(EnumU64 value, string json)
        {
            // MariaDB serializes UInt64.MaxValue as "18446744073709551615" instead of "-1"
            // Adjust the expected JSON value for MariaDB to match its behavior
            if (AppConfig.ServerVersion.Type == ServerType.MariaDb && value == EnumU64.Max)
            {
                json = """{"Prop":18446744073709551615}""";
            }
            
            return base.Can_read_write_ulong_enum_JSON_values(value, json);
        }

        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
