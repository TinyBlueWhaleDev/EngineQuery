using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.Metadata.Resolvers
{
    /// <summary>
    /// Resolves entity metadata using table and column attributes.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="TableAttribute"/> and <see cref="ColumnAttribute"/> when available.
    /// Falls back to CLR type and property names when attributes are not defined.
    /// </remarks>
    public sealed class AttributeEntityMetadataResolver : IEntityMetadataResolver
    {
        /// <summary>
        /// Resolves metadata associated with the specified entity type using mapping attributes.
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
            var tableAttribute = entityType.GetCustomAttribute<TableAttribute>();

            var properties = entityType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanRead)
                .ToDictionary(
                    property => property.Name,
                    property =>
                    {
                        var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();

                        return new EntityPropertyMetadata
                        {
                            PropertyName = property.Name,
                            ColumnName = columnAttribute?.Name ?? property.Name
                        };
                    });

            metadata = new EntityMetadata
            {
                EntityType = entityType,
                TableName = tableAttribute?.Name ?? entityType.Name,
                Properties = properties
            };

            return true;
        }
    }
}
