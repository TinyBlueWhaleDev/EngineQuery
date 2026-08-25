using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Helpers;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
{
    /// <summary>
    /// Builds SQL FROM clauses from query source definitions.
    /// </summary>
    /// <remarks>
    /// This builder supports physical root tables and derived table query sources.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="FromClauseBuilder"/> class.
    /// </remarks>
    /// <param name="subqueryCompiler">
    /// Subquery compiler used to compile derived table sources.
    /// </param>
    public sealed class FromClauseBuilder(SubqueryCompiler subqueryCompiler) : IRequiredSqlClauseBuilder
    {
        private readonly SubqueryCompiler _subqueryCompiler = subqueryCompiler ?? throw new ArgumentNullException(nameof(subqueryCompiler));

        /// <summary>
        /// Builds the SQL FROM clause.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition that contains source metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL FROM clause.
        /// </returns>
        public string Build(
            CompiledQueryDefinition queryDefinition,
            QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var rootSource = queryDefinition.SourceDefinitions.TryGetValue(queryDefinition.EntityType, out var sourceDefinition)
                ? sourceDefinition
                : null;

            if (rootSource is not null)
                return $"FROM {BuildQuerySourceReference(rootSource, context)}";

            var tableName = SqlIdentifierHelper.BuildTableReference(context.DatabaseDialect, queryDefinition.TableName, queryDefinition.SchemaName);

            return string.IsNullOrWhiteSpace(queryDefinition.TableAlias)
                ? $"FROM {tableName}"
                : $"FROM {tableName} AS {context.DatabaseDialect.EscapeIdentifier(queryDefinition.TableAlias)}";
        }

        private string BuildQuerySourceReference(QuerySourceDefinition sourceDefinition, QueryCompilationContext context)
        {
            if (sourceDefinition.IsDerivedTable)
            {
                var commandText = _subqueryCompiler.CompileAndReindex(
                    sourceDefinition.Subquery!,
                    context);

                return $"({commandText}) AS {context.DatabaseDialect.EscapeIdentifier(sourceDefinition.TableAlias)}";
            }

            if (sourceDefinition.IsTable)
            {
                var tableName = SqlIdentifierHelper.BuildTableReference(context.DatabaseDialect, sourceDefinition.TableName!, sourceDefinition.SchemaName);

                return $"{tableName} AS {context.DatabaseDialect.EscapeIdentifier(sourceDefinition.TableAlias)}";
            }

            throw new InvalidOperationException("Query source must define either a physical table or a derived table subquery.");
        }
    }
}
