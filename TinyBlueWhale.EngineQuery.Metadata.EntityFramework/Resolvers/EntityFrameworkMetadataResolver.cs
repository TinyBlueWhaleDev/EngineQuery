using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.Metadata.EntityFramework.Resolvers
{

    /// <summary>
    /// Resolves EngineQuery entity metadata from an Entity Framework Core model.
    /// </summary>
    /// <param name="model">
    /// Entity Framework Core model.
    /// </param>
    /// <param name="options">
    /// Metadata resolver options.
    /// </param>
    public sealed class EntityFrameworkMetadataResolver(IModel model, EntityFrameworkMetadataResolverOptions options) : IEntityMetadataResolver
    {
        private readonly IModel _model = model ?? throw new ArgumentNullException(nameof(model));
        private readonly EntityFrameworkMetadataResolverOptions _options = options ?? throw new ArgumentNullException(nameof(options));

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkMetadataResolver"/>
        /// class using the default resolver options.
        /// </summary>
        /// <param name="model">
        /// Entity Framework Core model.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="model"/> is null.
        /// </exception>
        public EntityFrameworkMetadataResolver(IModel model)
            : this(model, EntityFrameworkMetadataResolverOptions.Default)
        {
        }

        /// <summary>
        /// Attempts to resolve metadata associated with the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the metadata.
        /// </typeparam>
        /// <param name="metadata">
        /// Resolved entity metadata when available.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when metadata is resolved successfully;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryResolve<TEntity>(out EntityMetadata? metadata)
        {
            var entityType = typeof(TEntity);

            var efEntityType = _model.FindEntityType(entityType);

            if (efEntityType is null)
            {
                metadata = null;

                return false;
            }

            var tableName = efEntityType.GetTableName();

            if (string.IsNullOrWhiteSpace(tableName))
            {
                metadata = null;

                return false;
            }

            var schema = efEntityType.GetSchema();

            var storeObjectIdentifier = StoreObjectIdentifier.Table(
                tableName,
                schema);

            var properties = new Dictionary<string, EntityPropertyMetadata>();

            foreach (var property in efEntityType.GetProperties())
            {
                if (!_options.IncludeShadowProperties &&
                    property.IsShadowProperty())
                    continue;

                var columnName = property.GetColumnName(storeObjectIdentifier);

                if (string.IsNullOrWhiteSpace(columnName))
                    continue;

                properties[property.Name] = new EntityPropertyMetadata
                {
                    PropertyName = property.Name,
                    ColumnName = columnName
                };
            }

            metadata = new EntityMetadata
            {
                EntityType = entityType,
                SchemaName = schema,
                TableName = tableName,
                Properties = properties
            };

            return true;
        }
    }
}
