using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using System.Linq.Expressions;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Projections
{    

    /// <summary>
    /// Builds aggregate SELECT projection definitions.
    /// </summary>
    internal sealed class AggregateProjectionBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);

        /// <summary>
        /// Adds an aggregate projection for an entity available in the current query scope.
        /// </summary>
        public void Add<TEntity>(QueryAggregateFunction function, Expression<Func<TEntity, object>> selector, string alias)
        {
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>();

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
    }
}
