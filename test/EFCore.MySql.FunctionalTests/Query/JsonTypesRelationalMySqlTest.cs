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
        [SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.SpatialJsonSupport))]
        public override Task Can_read_write_line_string()
            => base.Can_read_write_line_string();

        [SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.SpatialJsonSupport))]
        public override Task Can_read_write_point()
            => base.Can_read_write_point();

        [SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.SpatialJsonSupport))]
        public override Task Can_read_write_polygon()
            => base.Can_read_write_polygon();

        [SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.SpatialJsonSupport))]
        public override Task Can_read_write_multi_line_string()
            => base.Can_read_write_multi_line_string();

        // Skip ulong enum test - MariaDB serializes UInt64 Max differently
        [ConditionalFact(Skip = "MariaDB 10.6+ serializes UInt64.MaxValue as full number instead of -1")]
        public override Task Can_read_write_collection_of_nullable_ulong_enum_JSON_values()
            => Task.CompletedTask;

        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
