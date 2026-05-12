using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.Enums;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a SQL JOIN definition associated with a query.
    /// </summary>
    public sealed record QueryJoinDefinition
    {
        /// <summary>
        /// Gets the SQL join type.
        /// </summary>
        public required QueryJoinType JoinType { get; init; }

        /// <summary>
        /// Gets the joined table name.
        /// </summary>
        public required string TableName { get; init; }

        /// <summary>
        /// Gets the joined table alias.
        /// </summary>
        public required string TableAlias { get; init; }

        /// <summary>
        /// Gets the CLR source entity type associated with the join source.
        /// </summary>
        public required Type SourceType { get; init; }

        /// <summary>
        /// Gets the CLR joined entity type.
        /// </summary>
        public required Type JoinTypeEntity { get; init; }

        /// <summary>
        /// Gets the join predicate expression.
        /// </summary>
        public required LambdaExpression JoinExpression { get; init; }

        /// <summary>
        /// Gets the source table alias used in the join condition.
        /// </summary>
        public required string SourceAlias { get; init; }

        /// <summary>
        /// Gets the property-to-column mappings for the source entity used in the join condition.
        /// </summary>
        public required IReadOnlyDictionary<string, string> SourceColumnMappings { get; init; }

        /// <summary>
        /// Gets the property-to-column mappings for the joined entity used in the join condition.
        /// </summary>
        public required IReadOnlyDictionary<string, string> JoinColumnMappings { get; init; }
    }
}
