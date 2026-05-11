using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.Metadata.Resolvers
{
    /// <summary>
    /// Resolves entity metadata using CLR type and property names as database names.
    /// </summary>
    /// <remarks>
    /// This resolver does not apply pluralization or naming transformations.
    /// The entity type name becomes the table name and each property name becomes its column name.
    /// </remarks>
    public sealed class ConventionEntityMetadataResolver : IEntityMetadataResolver
    {
        /// <summary>
        /// Resolves metadata associated with the specified entity type using naming conventions.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the metadata.
        /// </typeparam>
        /// <returns>
        /// Resolved entity metadata.
        /// </returns>
        public bool TryResolve<TEntity>(out EntityMetadata? metadata)
        {
            var entityType = typeof(TEntity);

            var properties = entityType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanRead)
                .ToDictionary(
                    property => property.Name,
                    property => new EntityPropertyMetadata
                    {
                        PropertyName = property.Name,
                        ColumnName = property.Name
                    });

            metadata = new EntityMetadata
            {
                EntityType = entityType,
                TableName = entityType.Name,
                Properties = properties
            };

            return true;
        }
    }
}
