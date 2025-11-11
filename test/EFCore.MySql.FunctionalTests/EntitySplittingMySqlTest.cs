using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests;

public class EntitySplittingMySqlTest(EntitySplittingMySqlTest.EntitySplittingMySqlFixture fixture, ITestOutputHelper testOutputHelper)
    : EntitySplittingTestBase(fixture, testOutputHelper)
{
    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;

    public class EntitySplittingMySqlFixture : NonSharedModelTestFixture
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        protected override string StoreName
            => "EntitySplittingTest";
    }
}
