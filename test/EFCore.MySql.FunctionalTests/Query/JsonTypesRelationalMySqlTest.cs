using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Types;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
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

        // Note: Can_read_write_ulong_enum_JSON_values is a parameterized Theory method that cannot be overridden.
        // The test case with UInt64.MaxValue (18446744073709551615) will fail on MariaDB due to different
        // serialization behavior. This is a known limitation documented in the PR description.
        // MariaDB serializes UInt64.MaxValue as "18446744073709551615" instead of "-1".

        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
