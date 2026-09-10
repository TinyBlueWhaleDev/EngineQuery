using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Projection;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding
{

    /// <summary>
    /// Builds SQL window function definitions.
    /// </summary>
    internal sealed class WindowFunctionBuilder(Func<Type, QuerySourceDefinition> sourceResolver) : IWindowFunctionBuilder
    {
        private readonly Func<Type, QuerySourceDefinition> _sourceResolver = sourceResolver ?? throw new ArgumentNullException(nameof(sourceResolver));
        private readonly List<QueryWindowPartitionDefinition> _partitions = [];
        private readonly List<QueryWindowOrderingDefinition> _orderings = [];

        /// <summary>
        /// Adds a PARTITION BY column to the window function.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the partition column.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the column included in the PARTITION BY clause.
        /// </param>
        /// <returns>
        /// Current window function builder instance.
        /// </returns>
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
        /// <typeparam name="TEntity">
        /// Entity type associated with the ordered column.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the column included in the window ORDER BY clause.
        /// </param>
        /// <returns>
        /// Current window function builder instance.
        /// </returns>
        public IWindowFunctionBuilder OrderBy<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            return AddOrdering(selector, QueryOrderingDirection.Ascending);
        }

        /// <summary>
        /// Adds a descending ORDER BY column to the window function.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the ordered column.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the column included in the window ORDER BY clause.
        /// </param>
        /// <returns>
        /// Current window function builder instance.
        /// </returns>
        public IWindowFunctionBuilder OrderByDescending<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            return AddOrdering(selector, QueryOrderingDirection.Descending);
        }

        /// <summary>
        /// Builds a SQL window function definition from the configured window state.
        /// </summary>
        /// <param name="function">
        /// Window function represented by the definition.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the window function projection.
        /// </param>
        /// <param name="arguments">
        /// Optional arguments supplied to the window function.
        /// </param>
        /// <returns>
        /// Window function definition containing the configured partitions,
        /// orderings and arguments.
        /// </returns>
        internal QueryWindowFunctionDefinition BuildWindowFunctionDefinition(QueryWindowFunction function, string alias, IReadOnlyList<QueryWindowFunctionArgumentDefinition>? arguments = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            if (_orderings.Count == 0)
                throw new InvalidOperationException("Window functions require at least one ORDER BY column.");

            return new QueryWindowFunctionDefinition
            {
                Function = function,
                Alias = alias,
                Partitions = _partitions,
                Orderings = _orderings,
                Arguments = arguments ?? []
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
