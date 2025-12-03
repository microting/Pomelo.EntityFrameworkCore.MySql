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
    // Currently, GroupBy over JSON collections of structural types is not supported.
    // The MySQL provider's TransformJsonQueryToTable method throws InvalidOperationException because
    // EF Core's SelectExpression.AddCrossJoin does not properly generate the CROSS JOIN keyword when
    // combining table expressions with JSON_TABLE functions, resulting in invalid SQL syntax.
    // See MySqlQueryableMethodTranslatingExpressionVisitor.TransformJsonQueryToTable (line 248).
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
