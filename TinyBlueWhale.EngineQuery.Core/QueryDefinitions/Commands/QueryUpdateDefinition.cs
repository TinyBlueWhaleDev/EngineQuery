namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Commands
{
    /// <summary>
    /// Represents the UPDATE-specific intent associated with a compiled SQL command.
    /// </summary>
    public sealed class QueryUpdateDefinition
    {
        /// <summary>
        /// Gets the column assignments associated with the UPDATE command.
        /// </summary>
        public List<QueryUpdateAssignmentDefinition> AssignmentDefinitions { get; } = [];
    }
}
