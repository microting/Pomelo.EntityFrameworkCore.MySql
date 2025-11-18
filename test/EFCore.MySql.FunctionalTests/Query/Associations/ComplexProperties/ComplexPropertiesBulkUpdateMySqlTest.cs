using Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexProperties;

public class ComplexPropertiesBulkUpdateMySqlTest : ComplexPropertiesBulkUpdateTestBase<ComplexPropertiesBulkUpdateMySqlTest.ComplexPropertiesBulkUpdateMySqlFixture>
{
    public ComplexPropertiesBulkUpdateMySqlTest(ComplexPropertiesBulkUpdateMySqlFixture fixture)
        : base(fixture)
    {
    }

    public class ComplexPropertiesBulkUpdateMySqlFixture : ComplexPropertiesFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
