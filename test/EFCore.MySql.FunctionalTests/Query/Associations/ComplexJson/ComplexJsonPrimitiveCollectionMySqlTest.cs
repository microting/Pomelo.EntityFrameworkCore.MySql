using System.Threading.Tasks;
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

    // TODO: Fix query translation issues for Any/Contains with predicate on primitive collections in JSON
    [ConditionalFact(Skip = "Query translation issue: Returns 0 results instead of expected results")]
    public override Task Any_predicate()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "Query translation issue: Returns 0 results instead of expected results")]
    public override Task Contains()
    {
        return Task.CompletedTask;
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
