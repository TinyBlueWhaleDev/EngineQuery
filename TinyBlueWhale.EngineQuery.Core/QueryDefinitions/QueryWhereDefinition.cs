using System.Linq.Expressions;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a filtering definition used to generate SQL WHERE conditions.
    /// </summary>
    public sealed record QueryWhereDefinition()
    {
        /// <summary>
        /// Gets the predicate expression associated with the filter definition.
        /// </summary>
        public LambdaExpression PredicateExpression { get; init; } = null!;

        /// <summary>
        /// Gets the CLR entity type that owns the filtered property.
        /// </summary>
        public Type? SourceType { get; init; }

        /// <summary>
        /// Gets the table alias associated with the filtered source.
        /// </summary>
        public string? SourceAlias { get; init; }

        /// <summary>
        /// Gets the property-to-column mappings associated with the filtered source.
        /// </summary>
        public IReadOnlyDictionary<string, string>? SourceColumnMappings { get; init; }
    }
}
