using Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexProperties;

public class ComplexPropertiesMiscellaneousMySqlTest : ComplexPropertiesMiscellaneousTestBase<ComplexPropertiesMiscellaneousMySqlTest.ComplexPropertiesMiscellaneousMySqlFixture>
{
    public ComplexPropertiesMiscellaneousMySqlTest(ComplexPropertiesMiscellaneousMySqlFixture fixture)
        : base(fixture)
    {
    }

    public class ComplexPropertiesMiscellaneousMySqlFixture : ComplexPropertiesFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
