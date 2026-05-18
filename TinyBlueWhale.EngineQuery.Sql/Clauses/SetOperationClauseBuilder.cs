using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
{
    /// <summary>
    /// Builds SQL set operation clauses such as UNION, UNION ALL, INTERSECT and EXCEPT.
    /// </summary>
    /// <remarks>
    /// This builder appends set operation queries to an already-built base command text.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SetOperationClauseBuilder"/> class.
    /// </remarks>
    /// <param name="subqueryCompiler">
    /// Subquery compiler used to compile set operation queries.
    /// </param>
    public sealed class SetOperationClauseBuilder(SubqueryCompiler subqueryCompiler)
    {
        private readonly SubqueryCompiler _subqueryCompiler = subqueryCompiler ?? throw new ArgumentNullException(nameof(subqueryCompiler));

        /// <summary>
        /// Determines whether set operation clauses should be built.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when set operations are configured; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool CanBuild(CompiledQueryDefinition queryDefinition)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);

            return queryDefinition.SetOperationDefinitions.Count > 0;
        }

        /// <summary>
        /// Appends SQL set operation clauses to the specified command text.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition that contains set operation metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <param name="commandText">
        /// Base SQL command text.
        /// </param>
        /// <returns>
        /// SQL command text with set operations appended.
        /// </returns>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context, string commandText)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var builder = new StringBuilder(commandText);

            foreach (var setOperationDefinition in queryDefinition.SetOperationDefinitions)
            {
                var setOperationCommandText = _subqueryCompiler.CompileAndReindex(
                    setOperationDefinition.Query,
                    context);

                var setOperationKeyword = ResolveSetOperationKeyword(setOperationDefinition.Operation);

                builder
                    .AppendLine()
                    .AppendLine(setOperationKeyword)
                    .Append(setOperationCommandText);
            }

            return builder.ToString();
        }

        private static string ResolveSetOperationKeyword(QuerySetOperation operation)
        {
            return operation switch
            {
                QuerySetOperation.Union => "UNION",
                QuerySetOperation.UnionAll => "UNION ALL",
                QuerySetOperation.Intersect => "INTERSECT",
                QuerySetOperation.Except => "EXCEPT",
                _ => throw new NotSupportedException($"Set operation '{operation}' is not supported.")
            };
        }
    }
}
