using Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexJson;

// Re-enabled to test JSON functionality for complex types
//
// TODO: All tests in this class should be skipped for MariaDB.
// 
// MariaDB stores JSON as LONGTEXT and doesn't support CAST(... AS json). When comparing JSON to NULL, 
// we need to distinguish between database NULL vs JSON null value ("null"), which requires CAST AS json 
// on MySQL but isn't supported on MariaDB. This causes incorrect result counts (returns 7 instead of 1) 
// because the NULL comparison semantics differ without the CAST.
//
// Until proper JSON-null-vs-database-NULL handling is implemented for MariaDB's LONGTEXT-based JSON storage,
// all structural equality tests in this class will fail on MariaDB with JsonDataTypeEmulation enabled.
//
// To skip these tests for MariaDB in CI/test runs, configure the test runner to skip tests with the
// "SkipForMariaDb" trait, or run tests against MySQL only.
//
// See: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/151
[Trait("Category", "SkipForMariaDb")]
public class ComplexJsonStructuralEqualityMySqlTest : ComplexJsonStructuralEqualityRelationalTestBase<ComplexJsonStructuralEqualityMySqlTest.ComplexJsonStructuralEqualityMySqlFixture>
{
    public ComplexJsonStructuralEqualityMySqlTest(ComplexJsonStructuralEqualityMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    public class ComplexJsonStructuralEqualityMySqlFixture : ComplexJsonRelationalFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
