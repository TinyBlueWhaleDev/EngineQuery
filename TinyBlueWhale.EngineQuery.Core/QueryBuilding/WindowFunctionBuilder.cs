using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding
{

    /// <summary>
    /// Builds SQL window function definitions.
    /// </summary>
    internal sealed class WindowFunctionBuilder(Func<Type, QuerySourceDefinition> sourceResolver) : IWindowFunctionBuilder
    {
        private readonly Func<Type, QuerySourceDefinition> _sourceResolver = sourceResolver;
        private readonly List<QueryWindowPartitionDefinition> _partitions = [];
        private readonly List<QueryWindowOrderingDefinition> _orderings = [];

        /// <summary>
        /// Adds a PARTITION BY column to the window function.
        /// </summary>
        public IWindowFunctionBuilder PartitionBy<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var column = QueryColumnExpressionExtractor
                .ExtractColumns(selector)
                .Single();

            _partitions.Add(
                new QueryWindowPartitionDefinition
                {
                    Column = column,
                    Source = _sourceResolver(typeof(TEntity))
                });

            return this;
        }

        /// <summary>
        /// Adds an ascending ORDER BY column to the window function.
        /// </summary>
        public IWindowFunctionBuilder OrderBy<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            return AddOrdering(selector, QueryOrderingDirection.Ascending);
        }

        /// <summary>
        /// Adds a descending ORDER BY column to the window function.
        /// </summary>
        public IWindowFunctionBuilder OrderByDescending<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            return AddOrdering(selector, QueryOrderingDirection.Descending);
        }

        internal QueryRowNumberDefinition BuildRowNumberDefinition(string alias)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            if (_orderings.Count == 0)
                throw new InvalidOperationException("ROW_NUMBER requires at least one ORDER BY column.");

            return new QueryRowNumberDefinition
            {
                Alias = alias,
                Partitions = _partitions,
                Orderings = _orderings
            };
        }

        // Adds an ordering column to the window function.
        private WindowFunctionBuilder AddOrdering<TEntity>(Expression<Func<TEntity, object>> selector, QueryOrderingDirection direction)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var column = QueryColumnExpressionExtractor
                .ExtractColumns(selector)
                .Single();

            _orderings.Add(
                new QueryWindowOrderingDefinition
                {
                    Column = column,
                    Direction = direction,
                    Source = _sourceResolver(typeof(TEntity))
                });

            return this;
        }
    }
}
