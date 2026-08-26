using System.Linq.Expressions;
using System.Reflection;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.Metadata.Fluent
{
    /// <summary>
    /// Provides a fluent API for configuring entity metadata.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Entity type being configured.
    /// </typeparam>
    public sealed class EntityMetadataBuilder<TEntity>
    {
        private readonly EntityMetadataRegistry _registry;
        private readonly Dictionary<string, EntityPropertyMetadata> _properties = [];
        private string? _schemaName;
        private string _tableName = typeof(TEntity).Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityMetadataBuilder{TEntity}"/> class.
        /// </summary>
        /// <param name="registry">
        /// Metadata registry where the configured entity metadata will be stored.
        /// </param>
        public EntityMetadataBuilder(EntityMetadataRegistry registry)
        {
            ArgumentNullException.ThrowIfNull(registry);

            _registry = registry;

            LoadDefaultProperties();
        }

        /// <summary>
        /// Configures the database table name associated with the entity.
        /// </summary>
        /// <param name="tableName">
        /// Database table name.
        /// </param>
        /// <returns>
        /// Current entity metadata builder instance.
        /// </returns>
        public EntityMetadataBuilder<TEntity> ToTable(string tableName, string? schemaName = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            if (schemaName is not null)
                ArgumentException.ThrowIfNullOrWhiteSpace(schemaName);

            _tableName = tableName;
            _schemaName = schemaName;
            Save();

            return this;
        }

        /// <summary>
        /// Creates a fluent property metadata builder for the selected entity property.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Selected property type.
        /// </typeparam>
        /// <param name="propertySelector">
        /// Expression that selects the entity property to configure.
        /// </param>
        /// <returns>
        /// Fluent property metadata builder.
        /// </returns>
        public PropertyMetadataBuilder<TEntity> Property<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector)
        {
            ArgumentNullException.ThrowIfNull(propertySelector);

            var propertyName = ExtractPropertyName(propertySelector);

            return new PropertyMetadataBuilder<TEntity>(this, propertyName);
        }

        /// <summary>
        /// Configures a column name for the specified entity property.
        /// </summary>
        /// <param name="propertyName">
        /// CLR property name.
        /// </param>
        /// <param name="columnName">
        /// Database column name.
        /// </param>
        internal void SetColumnName(string propertyName, string columnName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
            ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

            _properties[propertyName] = new EntityPropertyMetadata
            {
                PropertyName = propertyName,
                ColumnName = columnName
            };

            Save();
        }

        // Loads default property mappings using CLR property names as column names.
        private void LoadDefaultProperties()
        {
            var properties = typeof(TEntity)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanRead);

            foreach (var property in properties)
            {
                _properties[property.Name] = new EntityPropertyMetadata
                {
                    PropertyName = property.Name,
                    ColumnName = property.Name
                };
            }

            Save();
        }

        // Persists the current metadata snapshot into the registry.
        private void Save()
        {
            _registry.Register<TEntity>(
                new EntityMetadata
                {
                    EntityType = typeof(TEntity),
                    TableName = _tableName,
                    SchemaName = _schemaName,
                    Properties = _properties
                        .ToDictionary(
                            pair => pair.Key,
                            pair => pair.Value)
                });
        }

        // Extracts property names from direct member access expressions.
        private static string ExtractPropertyName<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            return expression.Body switch
            {
                MemberExpression memberExpression => memberExpression.Member.Name,

                UnaryExpression unaryExpression when unaryExpression.Operand
                    is MemberExpression memberExpression => memberExpression.Member.Name,

                _ => throw new NotSupportedException($"Expression '{expression}' is not supported as a property selector.")
            };
        }
    }
}
