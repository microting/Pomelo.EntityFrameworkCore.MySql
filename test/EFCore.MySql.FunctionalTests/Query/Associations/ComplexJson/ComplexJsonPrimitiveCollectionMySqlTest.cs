using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexJson;

// Re-enabled to test JSON functionality for complex types
public class ComplexJsonPrimitiveCollectionMySqlTest : ComplexJsonPrimitiveCollectionRelationalTestBase<ComplexJsonPrimitiveCollectionMySqlTest.ComplexJsonPrimitiveCollectionMySqlFixture>
{
    public ComplexJsonPrimitiveCollectionMySqlTest(ComplexJsonPrimitiveCollectionMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    public class ComplexJsonPrimitiveCollectionMySqlFixture : ComplexJsonRelationalFixtureBase
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
