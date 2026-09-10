using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
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

        /// <summary>
        /// Adds an ascending ORDER BY definition for the specified entity.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the selected columns.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the columns to order by.
        /// </param>
        public void AddAscending<T>(Expression<Func<T, object>> selector)
        {
            Add(selector, QueryOrderingDirection.Ascending);
        }

        /// <summary>
        /// Adds a descending ORDER BY definition for the specified entity.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the selected columns.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the columns to order by.
        /// </param>
        public void AddDescending<T>(Expression<Func<T, object>> selector)
        {
            Add(selector, QueryOrderingDirection.Descending);
        }

        /// <summary>
        /// Adds an ascending ORDER BY definition for an entity available
        /// in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected source columns.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the source columns to order by.
        /// </param>
        public void AddAscendingForSource<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            Add(selector, QueryOrderingDirection.Ascending);
        }

        /// <summary>
        /// Adds a descending ORDER BY definition for an entity available
        /// in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected source columns.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the source columns to order by.
        /// </param>
        public void AddDescendingForSource<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            Add(selector, QueryOrderingDirection.Descending);
        }

        /// <summary>
        /// Adds an ORDER BY definition using the specified ordering direction.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected columns.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the columns to order by.
        /// </param>
        /// <param name="direction">
        /// Ordering direction applied to the selected columns.
        /// </param>
        private void Add<TEntity>(Expression<Func<TEntity, object>> selector, QueryOrderingDirection direction)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var sourceDefinition =
                _sourceResolver.Resolve<TEntity>(selector.Parameters.Single());

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
