using System;
using System.Threading.Tasks;
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

    // TODO: Remove this skip once TransformJsonQueryToTable is fully implemented for MySQL.
    // Currently, SelectMany over JSON collections of structural types is not supported due to
    // EF Core query assembly issues. See MySqlQueryableMethodTranslatingExpressionVisitor.TransformJsonQueryToTable.
    // The implementation throws InvalidOperationException with a clear message about the limitation.
    // Related issue: #151
    [ConditionalTheory(Skip = "SelectMany over JSON collections of structural types not yet supported")]
    [MemberData(nameof(IsAsyncData))]
    public Task SelectMany_nested_collection_on_required_associate(bool async)
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
