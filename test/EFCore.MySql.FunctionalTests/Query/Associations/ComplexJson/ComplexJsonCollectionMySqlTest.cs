using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexJson;

// Re-enabled to test JSON functionality for complex types
public class ComplexJsonCollectionMySqlTest : ComplexJsonCollectionRelationalTestBase<ComplexJsonCollectionMySqlTest.ComplexJsonCollectionMySqlFixture>
{
    public ComplexJsonCollectionMySqlTest(ComplexJsonCollectionMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    // TODO: Remove this skip once TransformJsonQueryToTable is fully implemented for MySQL.
    // Currently, GroupBy over JSON collections of structural types is not supported due to
    // EF Core query assembly issues. The MySQL provider throws InvalidOperationException because
    // composing LINQ operators (such as GroupBy) over collections of structural types inside JSON
    // documents requires fixes in EF Core's query assembly logic or MySQL-specific SQL generation.
    // See MySqlQueryableMethodTranslatingExpressionVisitor.TransformJsonQueryToTable.
    [ConditionalFact(Skip = "GroupBy over JSON collections of structural types not yet supported")]
    public override Task GroupBy()
    {
        // This test is skipped because the feature is not yet implemented.
        return Task.CompletedTask;
    }

    public class ComplexJsonCollectionMySqlFixture : ComplexJsonRelationalFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
