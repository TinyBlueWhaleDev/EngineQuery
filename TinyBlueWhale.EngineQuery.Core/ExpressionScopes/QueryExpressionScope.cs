using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

namespace TinyBlueWhale.EngineQuery.Core.ExpressionScopes
{
    /// <summary>
    /// Represents the current expression parameter scope used during query parsing.
    /// </summary>
    public sealed class QueryExpressionScope
    {
        private readonly Dictionary<ParameterExpression, QuerySourceDefinition> _sources = [];

        /// <summary>
        /// Registers an expression parameter with its query source.
        /// </summary>
        /// <param name="parameterExpression">
        /// Expression parameter associated with the query source.
        /// </param>
        /// <param name="sourceDefinition">
        /// Query source registered for the expression parameter.
        /// </param>
        public void Register(ParameterExpression parameterExpression, QuerySourceDefinition sourceDefinition)
        {
            ArgumentNullException.ThrowIfNull(parameterExpression);
            ArgumentNullException.ThrowIfNull(sourceDefinition);

            _sources[parameterExpression] = sourceDefinition;
        }

        /// <summary>
        /// Resolves the query source associated with an expression parameter.
        /// </summary>
        /// <param name="parameterExpression">
        /// Expression parameter whose query source should be resolved.
        /// </param>
        /// <returns>
        /// Query source associated with the expression parameter.
        /// </returns>
        public QuerySourceDefinition Resolve(ParameterExpression parameterExpression)
        {
            ArgumentNullException.ThrowIfNull(parameterExpression);

            return _sources.TryGetValue(parameterExpression, out var sourceDefinition)
                ? sourceDefinition
                : throw new InvalidOperationException($"Parameter '{parameterExpression.Name}' is not registered in the current query expression scope.");
        }
    }
}
