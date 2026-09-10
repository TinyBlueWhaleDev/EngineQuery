using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Grouping;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Grouping
{


    /// <summary>
    /// Builds SQL HAVING definitions.
    /// </summary>
    internal sealed class HavingClauseBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);

        /// <summary>
        /// Adds an aggregate HAVING condition for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the aggregate expression.
        /// </typeparam>
        /// <param name="function">
        /// Aggregate function applied to the selected column.
        /// </param>
        /// <param name="selector">
        /// Expression that selects the column used by the aggregate function.
        /// </param>
        /// <param name="comparisonOperator">
        /// Comparison operator applied to the aggregate result.
        /// </param>
        /// <param name="value">
        /// Value compared against the aggregate result.
        /// </param>
        public void AddAggregate<TEntity>(
            QueryAggregateFunction function,
            Expression<Func<TEntity, object>> selector,
            QueryComparisonOperator comparisonOperator,
            object? value)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>(selector.Parameters[0]);

            var propertyName = QueryColumnExpressionExtractor
                .ExtractColumns(selector)
                .Single()
                .PropertyName;

            _context.QueryDefinition.HavingAggregateDefinitions.Add(
                new QueryHavingAggregateDefinition
                {
                    Function = function,
                    PropertyName = propertyName,
                    ComparisonOperator = comparisonOperator,
                    Value = value,
                    Source = sourceDefinition
                });
        }
    }
}
