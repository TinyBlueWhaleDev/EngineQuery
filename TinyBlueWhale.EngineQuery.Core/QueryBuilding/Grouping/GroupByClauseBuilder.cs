using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Grouping
{

    /// <summary>
    /// Builds SQL GROUP BY definitions.
    /// </summary>
    internal sealed class GroupByClauseBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);

        /// <summary>
        /// Adds a GROUP BY definition for an entity available in the current query scope.
        /// </summary>
        public void Add<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>();

            var columns = QueryColumnExpressionExtractor.ExtractColumns(selector);

            _context.QueryDefinition.GroupByDefinitions.Add(
                new QueryGroupByDefinition
                {
                    Source = sourceDefinition,
                    Columns = columns
                });
        }
    }
}
