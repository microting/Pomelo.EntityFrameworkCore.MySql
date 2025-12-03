using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexJson;

// Re-enabled to test JSON functionality for complex types
public class ComplexJsonSetOperationsMySqlTest : ComplexJsonSetOperationsRelationalTestBase<ComplexJsonSetOperationsMySqlTest.ComplexJsonSetOperationsMySqlFixture>
{
    public ComplexJsonSetOperationsMySqlTest(ComplexJsonSetOperationsMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    // TODO: Remove these skips once TransformJsonQueryToTable is fully implemented for MySQL.
    // The MySQL provider's TransformJsonQueryToTable method throws InvalidOperationException because
    // EF Core's SelectExpression.AddCrossJoin does not properly generate the CROSS JOIN keyword when
    // combining table expressions with JSON_TABLE functions, resulting in invalid SQL syntax.
    // See MySqlQueryableMethodTranslatingExpressionVisitor.TransformJsonQueryToTable (line 248).

    [ConditionalTheory(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    [MemberData(nameof(TrackingData))]
    public override Task Over_assocate_collection_Select_nested_with_aggregates_projected(QueryTrackingBehavior queryTrackingBehavior)
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Over_associate_collections()
    {
        return Task.CompletedTask;
    }

    [ConditionalFact(Skip = "LINQ operations over JSON collections of structural types not yet supported")]
    public override Task Over_nested_associate_collection()
    {
        return Task.CompletedTask;
    }

    public class ComplexJsonSetOperationsMySqlFixture : ComplexJsonRelationalFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
