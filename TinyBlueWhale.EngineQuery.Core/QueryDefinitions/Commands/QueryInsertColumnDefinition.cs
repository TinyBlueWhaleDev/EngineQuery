namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Commands
{
    /// <summary>
    /// Represents a target column associated with an INSERT command definition.
    /// </summary>
    public sealed record QueryInsertColumnDefinition
    {
        /// <summary>
        /// Gets the resolved target database column name.
        /// </summary>
        public required string ColumnName { get; init; }
    }
}
