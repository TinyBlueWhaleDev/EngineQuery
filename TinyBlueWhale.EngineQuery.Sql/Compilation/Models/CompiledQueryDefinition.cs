using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Sql.Compilation.Models
{
    /// <summary>
    /// Represents the internal query definition used by the SQL compiler.
    /// </summary>
    /// <remarks>
    /// This model captures query intent before SQL text is generated.
    /// It is not exposed to consumers of the public API.
    /// </remarks>
    public sealed class CompiledQueryDefinition
    {
        /// <summary>
        /// Gets or sets the source table name associated with the query.
        /// </summary>
        public required string TableName { get; set; }

        /// <summary>
        /// Gets the selected columns included in the query projection.
        /// </summary>
        public List<QuerySelectColumnDefinition> SelectDefinitions { get; } = [];

        /// <summary>
        /// Gets the filtering definitions used to generate SQL WHERE clauses.
        /// </summary>
        public List<QueryWhereDefinition> WhereDefinitions { get; } = [];

        /// <summary>
        /// Gets the ordering definitions used to generate SQL ORDER BY clauses.
        /// </summary>
        public List<QueryOrderingDefinition> OrderingDefinitions { get; } = [];

        /// <summary>
        /// Gets or sets the pagination definition used to generate SQL paging syntax.
        /// </summary>
        public QueryPaginationDefinition Pagination { get; set; } = new();

    }
}
