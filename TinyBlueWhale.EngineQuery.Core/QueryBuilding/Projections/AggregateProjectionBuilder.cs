using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Projection;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Projections
{

    /// <summary>
    /// Builds aggregate projections for the current query command.
    /// </summary>
    /// <param name="context">
    /// Query command builder context.
    /// </param>
    internal sealed class AggregateProjectionBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);

        /// <summary>
        /// Adds an aggregate projection for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type.
        /// </typeparam>
        /// <param name="function">
        /// Aggregate function.
        /// </param>
        /// <param name="selector">
        /// Aggregate selector expression.
        /// </param>
        /// <param name="alias">
        /// Aggregate projection alias.
        /// </param>
        public void Add<TEntity>(QueryAggregateFunction function, Expression<Func<TEntity, object>> selector, string alias)
        {
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>();

            if (QueryExpressionClassificationHelper.IsComputedExpression(selector.Body))
            {
                AddComputedAggregate(function, selector, alias, sourceDefinition);

                return;
            }

            AddColumnAggregate(function, selector, alias, sourceDefinition);
        }

        /// <summary>
        /// Adds a computed aggregate projection.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type.
        /// </typeparam>
        /// <param name="function">
        /// Aggregate function.
        /// </param>
        /// <param name="selector">
        /// Aggregate selector expression.
        /// </param>
        /// <param name="alias">
        /// Aggregate projection alias.
        /// </param>
        /// <param name="sourceDefinition">
        /// Query source definition.
        /// </param>
        private void AddComputedAggregate<TEntity>(QueryAggregateFunction function, Expression<Func<TEntity, object>> selector, string alias, QuerySourceDefinition sourceDefinition)
        {
            if (!SupportsComputedExpression(function))
                throw new NotSupportedException($"Aggregate function '{function}' does not support computed expressions.");

            _context.QueryDefinition.AggregateDefinitions.Add(
                new QueryAggregateDefinition
                {
                    Function = function,
                    Expression = selector,
                    Alias = alias,
                    Source = sourceDefinition
                });
        }

        /// <summary>
        /// Adds a column aggregate projection.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type.
        /// </typeparam>
        /// <param name="function">
        /// Aggregate function.
        /// </param>
        /// <param name="selector">
        /// Aggregate selector expression.
        /// </param>
        /// <param name="alias">
        /// Aggregate projection alias.
        /// </param>
        /// <param name="sourceDefinition">
        /// Query source definition.
        /// </param>
        private void AddColumnAggregate<TEntity>(QueryAggregateFunction function, Expression<Func<TEntity, object>> selector, string alias, QuerySourceDefinition sourceDefinition)
        {
            var propertyName = QueryColumnExpressionExtractor
                .ExtractColumns(selector)
                .Single()
                .PropertyName;

            _context.QueryDefinition.AggregateDefinitions.Add(
                new QueryAggregateDefinition
                {
                    Function = function,
                    PropertyName = propertyName,
                    Alias = alias,
                    Source = sourceDefinition
                });
        }

        /// <summary>
        /// Determines whether the specified aggregate function supports computed expressions.
        /// </summary>
        /// <param name="function">
        /// Aggregate function.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the aggregate function supports computed expressions; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool SupportsComputedExpression(QueryAggregateFunction function)
        {
            return function is QueryAggregateFunction.Sum
                or QueryAggregateFunction.Average
                or QueryAggregateFunction.Minimum
                or QueryAggregateFunction.Maximum;
        }
    }
}
