using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Ordering;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Projection;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Helpers;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
{
    /// <summary>
    /// Builds SQL ORDER BY clauses from query ordering definitions.
    /// </summary>
    /// <remarks>
    /// This builder preserves the fluent ordering sequence configured in the query definition.
    /// </remarks>
    public sealed class OrderByClauseBuilder : IOptionalSqlClauseBuilder
    {
        /// <summary>
        /// Determines whether an ORDER BY clause should be built.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when ordering definitions are configured; otherwise, <see langword="false"/>.
        /// </returns>
        public bool CanBuild(CompiledQueryDefinition queryDefinition)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);

            return queryDefinition.OrderingDefinitions.Count > 0;
        }

        /// <summary>
        /// Builds the SQL ORDER BY clause.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition that contains ordering metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL ORDER BY clause.
        /// </returns>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var orderingClauses = queryDefinition.OrderingDefinitions
                .SelectMany(orderingDefinition =>
                    BuildOrderingColumnReferences(context, orderingDefinition)
                        .Select(columnReference =>
                            $"{columnReference} {ResolveSqlOrderingDirection(orderingDefinition.Direction)}"));

            return "ORDER BY " + string.Join(", ", orderingClauses);
        }

        // Builds the column references associated with an ordering definition.
        private static IEnumerable<string> BuildOrderingColumnReferences(QueryCompilationContext context, QueryOrderingDefinition orderingDefinition)
        {
            foreach (var orderingColumn in orderingDefinition.Columns)
            {
                yield return BuildOrderingColumnReference(
                    context,
                    orderingDefinition,
                    orderingColumn);
            }
        }

        // Builds a column reference using the source captured by the ordering definition.
        private static string BuildOrderingColumnReference(QueryCompilationContext context, QueryOrderingDefinition orderingDefinition, QueryColumnDefinition orderingColumn)
        {
            return QueryColumnMappingHelper.ResolveColumnReference(
                orderingDefinition.Source,
                context.DatabaseDialect,
                orderingColumn.PropertyName);
        }

        // Resolves the SQL ordering direction.
        private static string ResolveSqlOrderingDirection(QueryOrderingDirection direction)
        {
            return direction == QueryOrderingDirection.Ascending ? "ASC" : "DESC";
        }
    }
}
