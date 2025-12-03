using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexJson;

// Re-enabled to test JSON functionality for complex types
public class ComplexJsonProjectionMySqlTest : ComplexJsonProjectionRelationalTestBase<ComplexJsonProjectionMySqlTest.ComplexJsonProjectionMySqlFixture>
{
    public ComplexJsonProjectionMySqlTest(ComplexJsonProjectionMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    // TODO: Remove these skips once TransformJsonQueryToTable is fully implemented for MySQL.
    // Currently, SelectMany over JSON collections of structural types is not supported due to
    // EF Core query assembly issues. The MySQL provider throws InvalidOperationException because
    // composing LINQ operators (such as SelectMany) over collections of structural types inside JSON
    // documents requires fixes in EF Core's query assembly logic or MySQL-specific SQL generation.
    // See MySqlQueryableMethodTranslatingExpressionVisitor.TransformJsonQueryToTable.

    [ConditionalTheory(Skip = "SelectMany over JSON collections of structural types not yet supported")]
    [MemberData(nameof(TrackingData))]
    public override Task SelectMany_associate_collection(QueryTrackingBehavior queryTrackingBehavior)
    {
        // This test is skipped because the feature is not yet implemented.
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = "SelectMany over JSON collections of structural types not yet supported")]
    [MemberData(nameof(TrackingData))]
    public override Task SelectMany_nested_collection_on_required_associate(QueryTrackingBehavior queryTrackingBehavior)
    {
        // This test is skipped because the feature is not yet implemented.
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = "SelectMany over JSON collections of structural types not yet supported")]
    [MemberData(nameof(TrackingData))]
    public override Task SelectMany_nested_collection_on_optional_associate(QueryTrackingBehavior queryTrackingBehavior)
    {
        // This test is skipped because the feature is not yet implemented.
        return Task.CompletedTask;
    }

    public class ComplexJsonProjectionMySqlFixture : ComplexJsonRelationalFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
