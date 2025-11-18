using Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexProperties;

public class ComplexPropertiesProjectionMySqlTest : ComplexPropertiesProjectionTestBase<ComplexPropertiesProjectionMySqlTest.ComplexPropertiesProjectionMySqlFixture>
{
    public ComplexPropertiesProjectionMySqlTest(ComplexPropertiesProjectionMySqlFixture fixture)
        : base(fixture)
    {
    }

    public class ComplexPropertiesProjectionMySqlFixture : ComplexPropertiesFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
