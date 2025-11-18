using Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexProperties;

public class ComplexPropertiesStructuralEqualityMySqlTest : ComplexPropertiesStructuralEqualityTestBase<ComplexPropertiesStructuralEqualityMySqlTest.ComplexPropertiesStructuralEqualityMySqlFixture>
{
    public ComplexPropertiesStructuralEqualityMySqlTest(ComplexPropertiesStructuralEqualityMySqlFixture fixture)
        : base(fixture)
    {
    }

    public class ComplexPropertiesStructuralEqualityMySqlFixture : ComplexPropertiesFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
