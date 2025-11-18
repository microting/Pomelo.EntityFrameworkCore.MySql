using Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexProperties;

public class ComplexPropertiesSetOperationsMySqlTest : ComplexPropertiesSetOperationsTestBase<ComplexPropertiesSetOperationsMySqlTest.ComplexPropertiesSetOperationsMySqlFixture>
{
    public ComplexPropertiesSetOperationsMySqlTest(ComplexPropertiesSetOperationsMySqlFixture fixture)
        : base(fixture)
    {
    }

    public class ComplexPropertiesSetOperationsMySqlFixture : ComplexPropertiesFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
