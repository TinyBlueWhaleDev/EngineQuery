using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context
{        

    /// <summary>
    /// Represents shared state used during query command construction.
    /// </summary>
    internal sealed class QueryCommandBuilderContext
    {
        /// <summary>
        /// Gets the SQL query compiler.
        /// </summary>
        public required IQueryCompiler QueryCompiler { get; init; }

        /// <summary>
        /// Gets the mutable query definition being constructed.
        /// </summary>
        public required CompiledQueryDefinition QueryDefinition { get; init; }

        /// <summary>
        /// Gets the metadata resolver associated with the query.
        /// </summary>
        public required IEntityMetadataResolver? MetadataResolver { get; init; }

        /// <summary>
        /// Gets the alias registry associated with the query scope.
        /// </summary>
        public required QueryAliasRegistry AliasRegistry { get; init; }

    }
}
