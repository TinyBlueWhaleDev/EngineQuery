using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Grouping;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Projection;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Helpers;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
{
    /// <summary>
    /// Builds SQL GROUP BY clauses from query grouping definitions.
    /// </summary>
    /// <remarks>
    /// This builder emits grouping columns using the query source captured by each
    /// grouping definition.
    /// </remarks>
    public sealed class GroupByClauseBuilder : IOptionalSqlClauseBuilder
    {
        /// <summary>
        /// Determines whether a GROUP BY clause should be built.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when grouping definitions are configured; otherwise, <see langword="false"/>.
        /// </returns>
        public bool CanBuild(CompiledQueryDefinition queryDefinition)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);

            return queryDefinition.GroupByDefinitions.Count > 0;
        }

        /// <summary>
        /// Builds the SQL GROUP BY clause.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition that contains grouping metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL GROUP BY clause.
        /// </returns>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var groupByClauses = queryDefinition.GroupByDefinitions
                .SelectMany(groupByDefinition =>
                    BuildGroupByColumnReferences(context, groupByDefinition));

            return "GROUP BY " + string.Join(", ", groupByClauses);
        }

        // Builds the column references associated with a grouping definition.
        private static IEnumerable<string> BuildGroupByColumnReferences(QueryCompilationContext context, QueryGroupByDefinition groupByDefinition)
        {
            foreach (var groupByColumn in groupByDefinition.Columns)
            {
                yield return BuildGroupByColumnReference(
                    context,
                    groupByDefinition,
                    groupByColumn);
            }
        }

        // Builds a column reference using the source captured by the grouping definition.
        private static string BuildGroupByColumnReference(QueryCompilationContext context, QueryGroupByDefinition groupByDefinition, QueryColumnDefinition groupByColumn)
        {
            return QueryColumnMappingHelper.ResolveColumnReference(
                groupByDefinition.Source,
                context.DatabaseDialect,
                groupByColumn.PropertyName);
        }
    }
}
