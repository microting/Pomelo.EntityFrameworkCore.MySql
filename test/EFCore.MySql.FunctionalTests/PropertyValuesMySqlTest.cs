using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class PropertyValuesMySqlTest : PropertyValuesTestBase<PropertyValuesMySqlTest.PropertyValuesMySqlFixture>
    {
        public PropertyValuesMySqlTest(PropertyValuesMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class PropertyValuesMySqlFixture : PropertyValuesFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                // Complex collections must be mapped to JSON columns in EF Core 10+
                modelBuilder.Entity<School>(b => b.ComplexCollection(e => e.Departments, b => b.ToJson()));

                // Ensure all JSON-mapped complex properties have the correct store type for MySQL
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var complexProperty in entityType.GetComplexProperties())
                    {
                        if (complexProperty.GetJsonPropertyName() != null)
                        {
                            complexProperty.ComplexType.SetContainerColumnType("json");
                        }
                    }
                }
            }
        }
    }
}
