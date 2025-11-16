using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.TransportationModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.GeneratedColumns))]
    public class TableSplittingMySqlTest(NonSharedFixture fixture, ITestOutputHelper testOutputHelper) : TableSplittingTestBase(fixture, testOutputHelper)
    {
        public TableSplittingMySqlTest(ITestOutputHelper testOutputHelper) : this(new NonSharedFixture(), testOutputHelper)
        {
        }

        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Engine>().ToTable("Vehicles")
                .Property(e => e.Computed).HasComputedColumnSql("1", stored: true);
        }
    }
}
