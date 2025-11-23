using Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexJson;

// Re-enabled to test JSON functionality for complex types
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
    }
}
