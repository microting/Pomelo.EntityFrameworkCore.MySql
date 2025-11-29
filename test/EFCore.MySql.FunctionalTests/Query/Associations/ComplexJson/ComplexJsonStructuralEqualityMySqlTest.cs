using Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexJson;

// Re-enabled to test JSON functionality for complex types
//
// TODO: The test Nested_associate_with_inline_null fails on MariaDB because MariaDB stores JSON as LONGTEXT
// and doesn't support CAST(... AS json). When comparing JSON to NULL, we need to distinguish between database 
// NULL vs JSON null value ("null"), which requires CAST AS json on MySQL but isn't supported on MariaDB. This 
// causes incorrect result counts (returns 7 instead of 1) because the NULL comparison semantics differ without 
// the CAST. The test should be skipped for MariaDB until proper JSON-null-vs-database-NULL handling is 
// implemented for LONGTEXT-based JSON storage.
// See: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/151
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
