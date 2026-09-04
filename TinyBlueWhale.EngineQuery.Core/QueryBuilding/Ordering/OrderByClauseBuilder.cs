using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Ordering;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Ordering
{
    /// <summary>
    /// Builds SQL ORDER BY definitions.
    /// </summary>
    internal sealed class OrderByClauseBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);

        public void AddAscending<T>(Expression<Func<T, object>> selector)
        {
            Add(selector, QueryOrderingDirection.Ascending);
        }

        public void AddDescending<T>(Expression<Func<T, object>> selector)
        {
            Add(selector, QueryOrderingDirection.Descending);
        }

        public void AddAscendingForSource<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            AddForSource(selector, QueryOrderingDirection.Ascending);
        }

        public void AddDescendingForSource<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            AddForSource(selector, QueryOrderingDirection.Descending);
        }

        private void Add<T>(Expression<Func<T, object>> selector, QueryOrderingDirection direction)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var sourceDefinition = ResolveOrderingSource<T>();

            AddColumns(selector, direction, sourceDefinition);
        }

        private void AddForSource<TEntity>(Expression<Func<TEntity, object>> selector, QueryOrderingDirection direction)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var sourceDefinition = ResolveOrderingSource<TEntity>();

            AddColumns(selector, direction, sourceDefinition);
        }

        /// <summary>
        /// Resolves the query source associated with an ordering definition.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type represented by the ordering selector.
        /// </typeparam>
        /// <returns>
        /// Query source associated with the ordering definition.
        /// </returns>
        private QuerySourceDefinition ResolveOrderingSource<TEntity>()
        {
            var rootSource = _context.QueryDefinition.RootSource;

            if (rootSource.EntityType == typeof(TEntity))
                return rootSource;

            return _sourceResolver.Resolve<TEntity>();
        }

        private void AddColumns<TEntity>(Expression<Func<TEntity, object>> selector, QueryOrderingDirection direction, QuerySourceDefinition sourceDefinition)
        {
            var columns = QueryColumnExpressionExtractor.ExtractColumns(selector);

            _context.QueryDefinition.OrderingDefinitions.Add(
                new QueryOrderingDefinition
                {
                    Direction = direction,
                    Source = sourceDefinition,
                    Columns = columns
                });
        }
    }
}
