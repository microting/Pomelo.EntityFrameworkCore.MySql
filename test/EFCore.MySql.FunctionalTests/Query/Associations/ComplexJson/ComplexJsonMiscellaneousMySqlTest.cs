using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexJson;

// Re-enabled to test JSON functionality for complex types
// Skip on MariaDB due to JsonDataTypeEmulation limitations
[SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.JsonDataTypeEmulation))]
public class ComplexJsonMiscellaneousMySqlTest : ComplexJsonMiscellaneousRelationalTestBase<ComplexJsonMiscellaneousMySqlTest.ComplexJsonMiscellaneousMySqlFixture>
{
    public ComplexJsonMiscellaneousMySqlTest(ComplexJsonMiscellaneousMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    public class ComplexJsonMiscellaneousMySqlFixture : ComplexJsonRelationalFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            var optionsBuilder = base.AddOptions(builder);
            new MySqlDbContextOptionsBuilder(optionsBuilder).EnablePrimitiveCollectionsSupport();
            return optionsBuilder;
        }
    }
}
