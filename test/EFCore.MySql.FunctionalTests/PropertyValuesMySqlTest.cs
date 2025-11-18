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
                // MySQL requires explicit column type specification for JSON columns
                modelBuilder.Entity<School>(b => b.ComplexCollection(e => e.Departments, b => b.ToJson().HasColumnType("json")));
            }
        }
    }
}
