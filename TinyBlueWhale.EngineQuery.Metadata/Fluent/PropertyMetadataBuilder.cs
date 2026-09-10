
namespace TinyBlueWhale.EngineQuery.Metadata.Fluent
{
    /// <summary>
    /// Provides a fluent API for configuring metadata associated with a single entity property.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Entity type that owns the configured property.
    /// </typeparam>
    public sealed class PropertyMetadataBuilder<TEntity>
    {
        private readonly EntityMetadataBuilder<TEntity> _entityBuilder;
        private readonly string _propertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMetadataBuilder{TEntity}"/> class.
        /// </summary>
        /// <param name="entityBuilder">
        /// Parent entity metadata builder.
        /// </param>
        /// <param name="propertyName">
        /// CLR property name being configured.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="entityBuilder"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="propertyName"/> is null or whitespace.
        /// </exception>
        public PropertyMetadataBuilder(EntityMetadataBuilder<TEntity> entityBuilder, string propertyName)
        {
            ArgumentNullException.ThrowIfNull(entityBuilder);
            ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

            _entityBuilder = entityBuilder;
            _propertyName = propertyName;
        }

        /// <summary>
        /// Configures the database column name associated with the selected property.
        /// </summary>
        /// <param name="columnName">
        /// Database column name.
        /// </param>
        /// <returns>
        /// Parent entity metadata builder instance.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="columnName"/> is null or whitespace.
        /// </exception>
        public EntityMetadataBuilder<TEntity> HasColumnName(string columnName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

            _entityBuilder.SetColumnName(_propertyName, columnName);

            return _entityBuilder;
        }
    }
}
