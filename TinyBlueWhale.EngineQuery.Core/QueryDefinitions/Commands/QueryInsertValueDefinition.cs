namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Commands
{
    /// <summary>
    /// Represents a value assigned to a target column by an INSERT command.
    /// </summary>
    public sealed record QueryInsertValueDefinition
    {
        /// <summary>
        /// Gets the resolved database column name associated with the inserted value.
        /// </summary>
        public required string ColumnName { get; init; }

        /// <summary>
        /// Gets or sets the value assigned to the target column.
        /// </summary>
        public object? Value { get; init; }
    }
}
