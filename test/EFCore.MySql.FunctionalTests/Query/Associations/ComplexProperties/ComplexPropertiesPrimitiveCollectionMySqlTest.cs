using Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexProperties;

// Disabled via internal access. Complex collections require JSON mapping which is not currently supported.
internal class ComplexPropertiesPrimitiveCollectionMySqlTest : ComplexPropertiesPrimitiveCollectionTestBase<ComplexPropertiesPrimitiveCollectionMySqlTest.ComplexPropertiesPrimitiveCollectionMySqlFixture>
{
    public ComplexPropertiesPrimitiveCollectionMySqlTest(ComplexPropertiesPrimitiveCollectionMySqlFixture fixture)
        : base(fixture)
    {
    }

    public class ComplexPropertiesPrimitiveCollectionMySqlFixture : ComplexPropertiesFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
