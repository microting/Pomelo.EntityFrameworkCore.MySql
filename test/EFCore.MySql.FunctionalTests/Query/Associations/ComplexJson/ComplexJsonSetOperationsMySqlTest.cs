using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Associations.ComplexJson;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexJson;

public class ComplexJsonSetOperationsMySqlTest : ComplexJsonSetOperationsRelationalTestBase<ComplexJsonSetOperationsMySqlTest.ComplexJsonSetOperationsMySqlTestFixture>
{
    public ComplexJsonSetOperationsMySqlTest(ComplexJsonSetOperationsMySqlTestFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    public class ComplexJsonSetOperationsMySqlTestFixture : ComplexJsonRelationalFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // Ensure all JSON-mapped complex properties have the correct store type for MySQL
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var complexProperty in entityType.GetComplexProperties())
                {
                    SetJsonStoreTypeRecursively(complexProperty);
                }
            }
        }

        private static void SetJsonStoreTypeRecursively(IMutableComplexProperty complexProperty)
        {
            if (complexProperty.GetJsonPropertyName() != null)
            {
                complexProperty.ComplexType.SetContainerColumnType("json");
            }

            // Also handle nested complex properties
            foreach (var nestedComplexProperty in complexProperty.ComplexType.GetComplexProperties())
            {
                SetJsonStoreTypeRecursively(nestedComplexProperty);
            }
        }
    }
}
