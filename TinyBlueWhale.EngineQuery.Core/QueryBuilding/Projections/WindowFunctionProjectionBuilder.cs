using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Projection;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Projections
{

    /// <summary>
    /// Builds SQL window function projections.
    /// </summary>
    internal sealed class WindowFunctionProjectionBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);

        /// <summary>
        /// Adds a LAG window function projection.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the projected value.
        /// </typeparam>
        /// <param name="expression">
        /// Expression that selects the value evaluated by the LAG function.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the LAG projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Delegate used to configure the window partitioning and ordering.
        /// </param>
        /// <param name="offset">
        /// Number of rows preceding the current row used by the LAG function.
        /// </param>
        public void AddLag<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder, int offset)
        {
            AddValueFunction(QueryWindowFunction.Lag, expression, alias, windowBuilder, offset);
        }

        /// <summary>
        /// Adds a LEAD window function projection.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the projected value.
        /// </typeparam>
        /// <param name="expression">
        /// Expression that selects the value evaluated by the LEAD function.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the LEAD projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Delegate used to configure the window partitioning and ordering.
        /// </param>
        /// <param name="offset">
        /// Number of rows following the current row used by the LEAD function.
        /// </param>
        public void AddLead<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder, int offset)
        {
            AddValueFunction(QueryWindowFunction.Lead, expression, alias, windowBuilder, offset);
        }

        /// <summary>
        /// Adds a FIRST_VALUE window function projection.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the projected value.
        /// </typeparam>
        /// <param name="expression">
        /// Expression that selects the value evaluated by the FIRST_VALUE function.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the FIRST_VALUE projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Delegate used to configure the window partitioning and ordering.
        /// </param>
        public void AddFirstValue<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            AddValueFunction(QueryWindowFunction.FirstValue, expression, alias, windowBuilder);
        }

        /// <summary>
        /// Adds a LAST_VALUE window function projection.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the projected value.
        /// </typeparam>
        /// <param name="expression">
        /// Expression that selects the value evaluated by the LAST_VALUE function.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the LAST_VALUE projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Delegate used to configure the window partitioning and ordering.
        /// </param>
        public void AddLastValue<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            AddValueFunction(QueryWindowFunction.LastValue, expression, alias, windowBuilder);
        }

        /// <summary>
        /// Adds an NTILE window function projection.
        /// </summary>
        /// <param name="buckets">
        /// Number of buckets used to distribute rows.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the NTILE projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Delegate used to configure the window partitioning and ordering.
        /// </param>
        public void AddNtile(int buckets, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(buckets);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);
            ArgumentNullException.ThrowIfNull(windowBuilder);

            var builder = new WindowFunctionBuilder(_sourceResolver.Resolve);

            windowBuilder(builder);

            var arguments = new List<QueryWindowFunctionArgumentDefinition>
            {
                new()
                {
                    ArgumentType = QueryWindowFunctionArgumentType.Constant,
                    ConstantValue = buckets
                }
            };

            _context.QueryDefinition.WindowFunctionDefinitions.Add(
                builder.BuildWindowFunctionDefinition(
                    QueryWindowFunction.Ntile,
                    alias,
                    arguments));
        }        

        /// <summary>
        /// Adds a ranking window function projection.
        /// </summary>
        /// <param name="function">
        /// Ranking window function represented by the projection.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the ranking projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Delegate used to configure the window partitioning and ordering.
        /// </param>
        public void AddRankingFunction(QueryWindowFunction function, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);
            ArgumentNullException.ThrowIfNull(windowBuilder);

            var builder = new WindowFunctionBuilder(_sourceResolver.Resolve);

            windowBuilder(builder);

            _context.QueryDefinition.WindowFunctionDefinitions.Add(
                builder.BuildWindowFunctionDefinition(
                    function,
                    alias));
        }

        /// <summary>
        /// Adds a value-based SQL window function projection.
        /// </summary>
        private void AddValueFunction<TEntity>(QueryWindowFunction function, Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder, int? offset = null)
        {
            ArgumentNullException.ThrowIfNull(expression);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);
            ArgumentNullException.ThrowIfNull(windowBuilder);

            if (offset.HasValue)
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(offset.Value);

            var source = _sourceResolver.Resolve<TEntity>(expression.Parameters.Single());

            var columns = QueryColumnExpressionExtractor.ExtractColumns(expression);

            if (columns.Count != 1)
                throw new InvalidOperationException("Window value functions require a single selected column.");

            var builder = new WindowFunctionBuilder(_sourceResolver.Resolve);

            windowBuilder(builder);

            var arguments = new List<QueryWindowFunctionArgumentDefinition>
            {
                new()
                {
                    ArgumentType = QueryWindowFunctionArgumentType.Column,
                    Column = columns[0],
                    Source = source
                }
            };

            if (offset.HasValue)
            {
                arguments.Add(
                    new QueryWindowFunctionArgumentDefinition
                    {
                        ArgumentType = QueryWindowFunctionArgumentType.Constant,
                        ConstantValue = offset.Value
                    });
            }

            _context.QueryDefinition.WindowFunctionDefinitions.Add(
                builder.BuildWindowFunctionDefinition(
                    function,
                    alias,
                    arguments));
        }
    }
}
