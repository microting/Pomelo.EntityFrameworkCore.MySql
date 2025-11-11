using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests;

public class EntitySplittingMySqlTest : EntitySplittingTestBase<EntitySplittingMySqlTest.EntitySplittingMySqlFixture>
{
    public EntitySplittingMySqlTest(EntitySplittingMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    public class EntitySplittingMySqlFixture : EntitySplittingFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
