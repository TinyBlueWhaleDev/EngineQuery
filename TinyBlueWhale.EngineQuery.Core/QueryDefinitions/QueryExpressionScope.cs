using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    public sealed class QueryExpressionScope
    {
        private readonly Dictionary<ParameterExpression, QuerySourceDefinition> _sources = [];

        /// <summary>
        /// Registers an expression parameter with its query source.
        /// </summary>
        public void Register(ParameterExpression parameterExpression, QuerySourceDefinition sourceDefinition)
        {
            ArgumentNullException.ThrowIfNull(parameterExpression);
            ArgumentNullException.ThrowIfNull(sourceDefinition);

            _sources[parameterExpression] = sourceDefinition;
        }

        /// <summary>
        /// Resolves the query source associated with an expression parameter.
        /// </summary>
        public QuerySourceDefinition Resolve(ParameterExpression parameterExpression)
        {
            ArgumentNullException.ThrowIfNull(parameterExpression);

            return _sources.TryGetValue(parameterExpression, out var sourceDefinition)
                ? sourceDefinition
                : throw new InvalidOperationException($"Parameter '{parameterExpression.Name}' is not registered in the current query expression scope.");
        }
    }
}
