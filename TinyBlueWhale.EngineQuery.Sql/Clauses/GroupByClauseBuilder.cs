using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Helpers;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
{
    /// <summary>
    /// Builds SQL GROUP BY clauses from query grouping definitions.
    /// </summary>
    /// <remarks>
    /// This builder emits grouping columns using either source-specific column mappings or the
    /// root query column mapping helper when no source alias is available.
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
                    BuildGroupByColumnReferences(queryDefinition, context, groupByDefinition));

            return "GROUP BY " + string.Join(", ", groupByClauses);
        }

        private static IEnumerable<string> BuildGroupByColumnReferences(
            CompiledQueryDefinition queryDefinition,
            QueryCompilationContext context,
            QueryGroupByDefinition groupByDefinition)
        {
            foreach (var groupByColumn in groupByDefinition.Columns)
            {
                yield return BuildGroupByColumnReference(
                    queryDefinition,
                    context,
                    groupByDefinition,
                    groupByColumn);
            }
        }

        private static string BuildGroupByColumnReference(
            CompiledQueryDefinition queryDefinition,
            QueryCompilationContext context,
            QueryGroupByDefinition groupByDefinition,
            QueryColumnDefinition groupByColumn)
        {
            if (!string.IsNullOrWhiteSpace(groupByDefinition.Source.TableAlias))
            {
                var columnName = SqlColumnReferenceBuilder.ResolveMappedColumnName(
                    groupByDefinition.Source.ColumnMappings,
                    groupByColumn.PropertyName);

                return context.DatabaseDialect.BuildQualifiedIdentifier(
                    groupByDefinition.Source.TableAlias,
                    columnName);
            }

            return QueryColumnMappingHelper.ResolveColumnReference(
                queryDefinition,
                context.DatabaseDialect,
                groupByColumn.PropertyName);
        }
    }
}
