using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests;

public class EntitySplittingMySqlTest(NonSharedFixture fixture, ITestOutputHelper testOutputHelper) : EntitySplittingTestBase(fixture, testOutputHelper)
{
    public EntitySplittingMySqlTest(ITestOutputHelper testOutputHelper) : this(new NonSharedFixture(), testOutputHelper)
    {
    }

    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;
}
