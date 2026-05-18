using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
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
                    BuildOrderingColumnReferences(queryDefinition, context, orderingDefinition)
                        .Select(columnReference =>
                            $"{columnReference} {ResolveSqlOrderingDirection(orderingDefinition.Direction)}"));

            return "ORDER BY " + string.Join(", ", orderingClauses);
        }

        private static IEnumerable<string> BuildOrderingColumnReferences(
            CompiledQueryDefinition queryDefinition,
            QueryCompilationContext context,
            QueryOrderingDefinition orderingDefinition)
        {
            foreach (var orderingColumn in orderingDefinition.Columns)
            {
                yield return BuildOrderingColumnReference(
                    queryDefinition,
                    context,
                    orderingDefinition,
                    orderingColumn);
            }
        }

        private static string BuildOrderingColumnReference(
            CompiledQueryDefinition queryDefinition,
            QueryCompilationContext context,
            QueryOrderingDefinition orderingDefinition,
            QueryColumnDefinition orderingColumn)
        {
            if (!string.IsNullOrWhiteSpace(orderingDefinition.Source.TableAlias))
            {
                var columnName = SqlColumnReferenceBuilder.ResolveMappedColumnName(orderingDefinition.Source.ColumnMappings, orderingColumn.PropertyName);

                return context.DatabaseDialect.BuildQualifiedIdentifier(orderingDefinition.Source.TableAlias, columnName);
            }

            return QueryColumnMappingHelper.ResolveColumnReference(queryDefinition, context.DatabaseDialect, orderingColumn.PropertyName);
        }

        private static string ResolveSqlOrderingDirection(QueryOrderingDirection direction)
        {
            return direction == QueryOrderingDirection.Ascending ? "ASC" : "DESC";
        }
    }
}
