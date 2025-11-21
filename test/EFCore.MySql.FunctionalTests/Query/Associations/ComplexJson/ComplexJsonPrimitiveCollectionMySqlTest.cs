using Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexJson;

public class ComplexJsonPrimitiveCollectionMySqlTest : ComplexJsonPrimitiveCollectionRelationalTestBase<ComplexJsonPrimitiveCollectionMySqlTest.ComplexJsonPrimitiveCollectionMySqlFixture>
{
    public ComplexJsonPrimitiveCollectionMySqlTest(ComplexJsonPrimitiveCollectionMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    public class ComplexJsonPrimitiveCollectionMySqlFixture : ComplexJsonRelationalFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
