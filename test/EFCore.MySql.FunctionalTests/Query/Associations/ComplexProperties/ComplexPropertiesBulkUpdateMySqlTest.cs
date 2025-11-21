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

public class ComplexPropertiesBulkUpdateMySqlTest : ComplexPropertiesBulkUpdateTestBase<ComplexPropertiesBulkUpdateMySqlTest.ComplexPropertiesBulkUpdateMySqlTestFixture>
{
    public ComplexPropertiesBulkUpdateMySqlTest(ComplexPropertiesBulkUpdateMySqlTestFixture fixture)
        : base(fixture)
    {
    }

    public class ComplexPropertiesBulkUpdateMySqlTestFixture : ComplexPropertiesFixtureBase
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

            // Also ensure all JSON-mapped complex properties have the correct store type for MySQL
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

        private static void ConfigureComplexCollectionAsJson(ModelBuilder modelBuilder, IMutableEntityType entityType, IMutableComplexProperty complexProperty)
        {
            try
            {
                // Get the member info (property or field)
                var memberInfo = complexProperty.PropertyInfo as MemberInfo ?? complexProperty.FieldInfo;
                if (memberInfo == null) return;

                // Get the collection element type
                var collectionType = complexProperty.ClrType;
                if (!collectionType.IsGenericType) return;
                
                var elementType = collectionType.GetGenericArguments().FirstOrDefault();
                if (elementType == null) return;

                // Get the EntityTypeBuilder for this entity type using the generic Entity<T>() method
                var entityMethod = typeof(ModelBuilder).GetMethods()
                    .FirstOrDefault(m => m.Name == nameof(ModelBuilder.Entity) &&
                                         m.IsGenericMethodDefinition &&
                                         m.GetParameters().Length == 0);
                if (entityMethod == null) return;

                var genericEntityMethod = entityMethod.MakeGenericMethod(entityType.ClrType);
                var entityBuilder = genericEntityMethod.Invoke(modelBuilder, null);
                if (entityBuilder == null) return;

                // Build the lambda expression: e => e.Property/Field
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var memberAccess = Expression.MakeMemberAccess(parameter, memberInfo);
                // Create the Func<TEntity, IEnumerable<TElement>> type
                var funcType = typeof(Func<,>).MakeGenericType(entityType.ClrType, collectionType);
                var expressionType = typeof(Expression<>).MakeGenericType(funcType);
                
                // Use the Expression.Lambda method that takes delegateType as first parameter
                var lambdaMethod = typeof(Expression).GetMethods()
                    .Where(m => m.Name == nameof(Expression.Lambda) && m.IsGenericMethod)
                    .Select(m => m.MakeGenericMethod(funcType))
                    .FirstOrDefault(m => m.GetParameters().Length == 2 &&
                                         m.GetParameters()[0].ParameterType == typeof(Expression) &&
                                         m.GetParameters()[1].ParameterType == typeof(ParameterExpression[]));
                
                if (lambdaMethod == null) return;
                
                var lambda = lambdaMethod.Invoke(null, new object[] { memberAccess, new[] { parameter } });
                if (lambda == null) return;

                // Find the ComplexCollection method - it's generic with one type parameter (the element type)
                var entityTypeBuilderType = typeof(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<>)
                    .MakeGenericType(entityType.ClrType);
                
                var complexCollectionMethod = entityTypeBuilderType.GetMethods()
                    .FirstOrDefault(m => m.Name == "ComplexCollection" && 
                                         m.IsGenericMethodDefinition && 
                                         m.GetGenericArguments().Length == 1 &&
                                         m.GetParameters().Length == 1);
                
                if (complexCollectionMethod == null) return;

                // Make the generic method with the element type
                var genericComplexCollectionMethod = complexCollectionMethod.MakeGenericMethod(elementType);
                
                var collectionBuilder = genericComplexCollectionMethod.Invoke(entityBuilder, new object[] { lambda });
                if (collectionBuilder == null) return;

                // Call ToJson() on the collection builder
                var toJsonMethod = collectionBuilder.GetType().GetMethod("ToJson", Type.EmptyTypes);
                if (toJsonMethod != null)
                {
                    toJsonMethod.Invoke(collectionBuilder, null);
                }
            }
            catch (Exception ex)
            {
                // Log the exception to help diagnose reflection issues
                Console.WriteLine($"Failed to configure complex collection {complexProperty.Name} on {entityType.ClrType.Name}: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
        }
    }
}
