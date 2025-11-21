using Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexJson;

// Disabled via internal access. JSON functionality is not currently supported.
internal class ComplexJsonBulkUpdateMySqlTest : ComplexJsonBulkUpdateRelationalTestBase<ComplexJsonBulkUpdateMySqlTest.ComplexJsonBulkUpdateMySqlFixture>
{
    public ComplexJsonBulkUpdateMySqlTest(ComplexJsonBulkUpdateMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    public class ComplexJsonBulkUpdateMySqlFixture : ComplexJsonRelationalFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
