using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;
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
    /// For INSERT SELECT commands, the SELECT source is resolved from the INSERT
    /// definition instead of the INSERT target root source.
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
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryDefinition"/> or
        /// <paramref name="context"/> is null.
        /// </exception>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var sourceDefinition = ResolveFromSource(queryDefinition);

            return $"FROM {BuildQuerySourceReference(sourceDefinition, context)}";
        }

        // Resolves the query source that represents the SQL FROM clause.
        private static QuerySourceDefinition ResolveFromSource(CompiledQueryDefinition queryDefinition)
        {
            if (queryDefinition.InsertDefinition?.SourceDefinition is not null)
                return queryDefinition.InsertDefinition.SourceDefinition;

            return queryDefinition.RootSource;
        }

        // Builds the SQL reference associated with the specified query source.
        private string BuildQuerySourceReference(QuerySourceDefinition sourceDefinition, QueryCompilationContext context)
        {
            if (sourceDefinition.IsDerivedTable)
            {
                if (string.IsNullOrWhiteSpace(sourceDefinition.TableAlias))
                    throw new InvalidOperationException("Derived table query sources require an alias.");

                var commandText = _subqueryCompiler.CompileAndReindex(sourceDefinition.Subquery!, context);

                return $"({commandText}) AS {context.DatabaseDialect.EscapeIdentifier(sourceDefinition.TableAlias)}";
            }

            if (sourceDefinition.IsTable)
            {
                var tableName = SqlIdentifierHelper.BuildTableReference(
                    context.DatabaseDialect,
                    sourceDefinition.TableName!,
                    sourceDefinition.SchemaName);

                return string.IsNullOrWhiteSpace(sourceDefinition.TableAlias)
                    ? tableName
                    : $"{tableName} AS {context.DatabaseDialect.EscapeIdentifier(sourceDefinition.TableAlias)}";
            }

            throw new InvalidOperationException("Query source must define either a physical table or a derived table subquery.");
        }
    }
}
