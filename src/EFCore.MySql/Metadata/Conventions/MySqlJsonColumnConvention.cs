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
    ///     A convention that configures the column type as "json" for properties and complex properties
    ///     that are mapped to JSON columns in the database.
    /// </summary>
    public class MySqlJsonColumnConvention : IModelFinalizingConvention
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
        public virtual void ProcessModelFinalizing(
            IConventionModelBuilder modelBuilder,
            IConventionContext<IConventionModelBuilder> context)
        {
            // Iterate through all entity types and their complex properties
            foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
            {
                foreach (var complexProperty in entityType.GetComplexProperties())
                {
                    // Check if this complex property is mapped to a JSON column
                    var jsonPropertyName = complexProperty.GetJsonPropertyName();
                    if (jsonPropertyName != null)
                    {
                        // Set the container column type to "json" for MySQL
                        var complexType = complexProperty.ComplexType;
                        if (complexType is IConventionComplexType conventionComplexType)
                        {
                            conventionComplexType.Builder.HasAnnotation(
                                RelationalAnnotationNames.ContainerColumnType,
                                "json",
                                fromDataAnnotation: false);
                        }
                    }
                }
            }
        }
    }
}
