// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.Metadata.Conventions
{
    /// <summary>
    ///     A convention that configures the column type as "json" for complex properties
    ///     that are mapped to JSON columns in the database.
    /// </summary>
    public class MySqlJsonColumnConvention : IComplexPropertyAddedConvention, IComplexPropertyAnnotationChangedConvention
    {
        /// <summary>
        ///     Creates a new instance of <see cref="MySqlJsonColumnConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        /// <param name="relationalDependencies"> Parameter object containing relational dependencies for this convention. </param>
        public MySqlJsonColumnConvention(
            ProviderConventionSetBuilderDependencies dependencies,
            RelationalConventionSetBuilderDependencies relationalDependencies)
        {
            Dependencies = dependencies;
            RelationalDependencies = relationalDependencies;
        }

        /// <summary>
        ///     Dependencies for this service.
        /// </summary>
        protected virtual ProviderConventionSetBuilderDependencies Dependencies { get; }

        /// <summary>
        ///     Relational provider-specific dependencies for this service.
        /// </summary>
        protected virtual RelationalConventionSetBuilderDependencies RelationalDependencies { get; }

        /// <inheritdoc />
        public virtual void ProcessComplexPropertyAdded(
            IConventionComplexPropertyBuilder propertyBuilder,
            IConventionContext<IConventionComplexPropertyBuilder> context)
        {
            SetJsonColumnTypeIfNeeded(propertyBuilder);
        }

        /// <inheritdoc />
        public virtual void ProcessComplexPropertyAnnotationChanged(
            IConventionComplexPropertyBuilder propertyBuilder,
            string name,
            IConventionAnnotation annotation,
            IConventionAnnotation oldAnnotation,
            IConventionContext<IConventionAnnotation> context)
        {
            // React when JsonPropertyName annotation is set (when .ToJson() is called)
            if (name == RelationalAnnotationNames.JsonPropertyName)
            {
                SetJsonColumnTypeIfNeeded(propertyBuilder);
            }
        }

        private static void SetJsonColumnTypeIfNeeded(IConventionComplexPropertyBuilder propertyBuilder)
        {
            var complexProperty = propertyBuilder.Metadata;
            
            // Check if this complex property is mapped to a JSON column
            // GetJsonPropertyName() returns non-null (can be empty string) when .ToJson() is called
            var jsonPropertyName = complexProperty.GetJsonPropertyName();
            if (jsonPropertyName != null)
            {
                // Set the container column type to "json" for MySQL
                // Use the proper extension method on the complex type (not property)
                var complexType = complexProperty.ComplexType;
                if (complexType is IConventionComplexType conventionComplexType)
                {
                    conventionComplexType.SetContainerColumnType("json", fromDataAnnotation: false);
                }
            }
        }
    }
}
