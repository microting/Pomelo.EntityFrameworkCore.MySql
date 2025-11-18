using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Associations.ComplexProperties;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.Associations.ComplexProperties;

public class ComplexPropertiesMiscellaneousMySqlTest : ComplexPropertiesMiscellaneousTestBase<ComplexPropertiesMiscellaneousMySqlTest.ComplexPropertiesMiscellaneousMySqlFixture>
{
    public ComplexPropertiesMiscellaneousMySqlTest(ComplexPropertiesMiscellaneousMySqlFixture fixture)
        : base(fixture)
    {
    }

    public class ComplexPropertiesMiscellaneousMySqlFixture : ComplexPropertiesFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // In EF Core 10+, complex collections must be mapped to JSON columns
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var complexProperty in entityType.GetComplexProperties())
                {
                    if (complexProperty.IsCollection)
                    {
                        // Use reflection to call ComplexCollection().ToJson() dynamically
                        ConfigureComplexCollectionAsJson(modelBuilder, entityType, complexProperty);
                    }
                }
            }
        }

        private static void ConfigureComplexCollectionAsJson(ModelBuilder modelBuilder, IMutableEntityType entityType, IMutableComplexProperty complexProperty)
        {
            // Get the EntityTypeBuilder for this entity type
            var entityMethod = typeof(ModelBuilder).GetMethod(nameof(ModelBuilder.Entity), 1, new[] { typeof(Type) });
            var entityBuilder = entityMethod.Invoke(modelBuilder, new object[] { entityType.ClrType });

            // Get the member info (property or field)
            var memberInfo = complexProperty.PropertyInfo as MemberInfo ?? complexProperty.FieldInfo;
            if (memberInfo == null) return;

            // Build the lambda expression: e => e.Property/Field
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var memberAccess = Expression.MakeMemberAccess(parameter, memberInfo);
            var lambda = Expression.Lambda(memberAccess, parameter);

            // Call ComplexCollection(lambda)
            var complexCollectionMethod = typeof(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<>)
                .MakeGenericType(entityType.ClrType)
                .GetMethods()
                .First(m => m.Name == "ComplexCollection" && m.GetParameters().Length == 1);

            var collectionBuilder = complexCollectionMethod.Invoke(entityBuilder, new object[] { lambda });

            // Call ToJson() on the collection builder
            var toJsonMethod = collectionBuilder.GetType().GetMethod("ToJson", new Type[] { });
            if (toJsonMethod != null)
            {
                toJsonMethod.Invoke(collectionBuilder, null);
            }
        }
    }
}
