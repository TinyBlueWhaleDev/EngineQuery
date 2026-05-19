using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Filtering
{
    /// <summary>
    /// Builds SQL WHERE clause definitions.
    /// </summary>
    internal sealed class WhereClauseBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);

        /// <summary>
        /// Adds a WHERE predicate for the root query entity.
        /// </summary>
        public void Add<T>(Expression<Func<T, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            var sourceDefinition = _sourceResolver.Resolve<T>();

            _context.QueryDefinition.WhereDefinitions.Add(
                new QueryWhereDefinition
                {
                    PredicateExpression = predicate,
                    Source = sourceDefinition
                });
        }

        /// <summary>
        /// Adds a WHERE predicate for an entity available in the current query scope.
        /// </summary>
        public void AddForSource<TEntity>(
            Expression<Func<TEntity, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>();

            _context.QueryDefinition.WhereDefinitions.Add(
                new QueryWhereDefinition
                {
                    PredicateExpression = predicate,
                    Source = sourceDefinition
                });
        }

        /// <summary>
        /// Adds a conditional WHERE predicate for the root query entity.
        /// </summary>
        public void AddIf<T>(bool condition, Expression<Func<T, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            if (!condition)
                return;

            Add(predicate);
        }

        /// <summary>
        /// Adds a conditional WHERE predicate for an entity available in the current query scope.
        /// </summary>
        public void AddIfForSource<TEntity>(bool condition, Expression<Func<TEntity, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            if (!condition)
                return;

            AddForSource(predicate);
        }

        /// <summary>
        /// Adds a scalar SQL function WHERE condition.
        /// </summary>
        public void AddFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object>> selector, QueryComparisonOperator comparisonOperator, object? value)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>();

            var propertyName = QueryColumnExpressionExtractor
                .ExtractColumns(selector)
                .Single()
                .PropertyName;

            _context.QueryDefinition.WhereScalarFunctionDefinitions.Add(
                new QueryWhereScalarFunctionDefinition
                {
                    Function = function,
                    PropertyName = propertyName,
                    ComparisonOperator = comparisonOperator,
                    Value = value,
                    Source = sourceDefinition
                });
        }

        /// <summary>
        /// Adds a computed WHERE expression for one entity source.
        /// </summary>
        public void AddComputed<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>();

            _context.QueryDefinition.WhereComputedExpressionDefinitions.Add(
                new QueryWhereComputedExpressionDefinition
                {
                    Expression = expression,
                    Sources = new Dictionary<ParameterExpression, QuerySourceDefinition>
                    {
                        [expression.Parameters[0]] = sourceDefinition
                    }
                });
        }

        /// <summary>
        /// Adds a computed WHERE expression for two entity sources.
        /// </summary>
        public void AddComputed<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var leftSource = _sourceResolver.Resolve<TLeft>();
            var rightSource = _sourceResolver.Resolve<TRight>();

            _context.QueryDefinition.WhereComputedExpressionDefinitions.Add(
                new QueryWhereComputedExpressionDefinition
                {
                    Expression = expression,
                    Sources = new Dictionary<ParameterExpression, QuerySourceDefinition>
                    {
                        [expression.Parameters[0]] = leftSource,
                        [expression.Parameters[1]] = rightSource
                    }
                });
        }
    }
}

