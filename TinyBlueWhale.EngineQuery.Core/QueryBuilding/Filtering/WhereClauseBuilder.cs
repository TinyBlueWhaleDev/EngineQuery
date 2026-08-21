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
        /// Adds a WHERE predicate for the root query entity using
        /// the default logical AND operator.
        /// </summary>
        /// <typeparam name="T">
        /// The root query entity type.
        /// </typeparam>
        /// <param name="predicate">
        /// The predicate expression to add.
        /// </param>
        public void Add<T>(Expression<Func<T, bool>> predicate)
        {
            Add(predicate, QueryLogicalOperator.And);
        }

        /// <summary>
        /// Adds a WHERE predicate for the root query entity using
        /// the specified logical operator.
        /// </summary>
        /// <typeparam name="T">
        /// The root query entity type.
        /// </typeparam>
        /// <param name="predicate">
        /// The predicate expression to add.
        /// </param>
        /// <param name="logicalOperator">
        /// The logical operator used to connect the predicate with
        /// the preceding predicate.
        /// </param>
        public void Add<T>(Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            var sourceDefinition = _sourceResolver.Resolve<T>();

            AddInternal(predicate, sourceDefinition, logicalOperator);
        }

        /// <summary>
        /// Adds a WHERE predicate for an entity available in the current
        /// query scope using the default logical AND operator.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The entity type associated with the predicate.
        /// </typeparam>
        /// <param name="predicate">
        /// The predicate expression to add.
        /// </param>
        public void AddForSource<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            AddForSource(predicate,QueryLogicalOperator.And);
        }

        /// <summary>
        /// Adds a WHERE predicate for an entity available in the current
        /// query scope using the specified logical operator.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The entity type associated with the predicate.
        /// </typeparam>
        /// <param name="predicate">
        /// The predicate expression to add.
        /// </param>
        /// <param name="logicalOperator">
        /// The logical operator used to connect the predicate with
        /// the preceding predicate.
        /// </param>
        public void AddForSource<TEntity>(Expression<Func<TEntity, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            var sourceDefinition = _sourceResolver.Resolve<TEntity>();

            AddInternal(predicate, sourceDefinition, logicalOperator);
        }

        /// <summary>
        /// Adds a conditional WHERE predicate for the root query entity
        /// using the default logical AND operator.
        /// </summary>
        /// <typeparam name="T">
        /// The root query entity type.
        /// </typeparam>
        /// <param name="condition">
        /// A value indicating whether the predicate should be added.
        /// </param>
        /// <param name="predicate">
        /// The predicate expression to add when the condition is true.
        /// </param>
        public void AddIf<T>(bool condition, Expression<Func<T, bool>> predicate)
        {
            AddIf(condition, predicate, QueryLogicalOperator.And);
        }

        /// <summary>
        /// Adds a conditional WHERE predicate for the root query entity
        /// using the specified logical operator.
        /// </summary>
        /// <typeparam name="T">
        /// The root query entity type.
        /// </typeparam>
        /// <param name="condition">
        /// A value indicating whether the predicate should be added.
        /// </param>
        /// <param name="predicate">
        /// The predicate expression to add when the condition is true.
        /// </param>
        /// <param name="logicalOperator">
        /// The logical operator used to connect the predicate with
        /// the preceding predicate.
        /// </param>
        public void AddIf<T>(bool condition, Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            if (!condition)
                return;

            Add(predicate, logicalOperator);
        }

        /// <summary>
        /// Adds a conditional WHERE predicate for an entity available
        /// in the current query scope using the default logical AND operator.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The entity type associated with the predicate.
        /// </typeparam>
        /// <param name="condition">
        /// A value indicating whether the predicate should be added.
        /// </param>
        /// <param name="predicate">
        /// The predicate expression to add when the condition is true.
        /// </param>
        public void AddIfForSource<TEntity>(bool condition, Expression<Func<TEntity, bool>> predicate)
        {
            AddIfForSource(condition, predicate, QueryLogicalOperator.And);
        }

        /// <summary>
        /// Adds a conditional WHERE predicate for an entity available
        /// in the current query scope using the specified logical operator.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The entity type associated with the predicate.
        /// </typeparam>
        /// <param name="condition">
        /// A value indicating whether the predicate should be added.
        /// </param>
        /// <param name="predicate">
        /// The predicate expression to add when the condition is true.
        /// </param>
        /// <param name="logicalOperator">
        /// The logical operator used to connect the predicate with
        /// the preceding predicate.
        /// </param>
        public void AddIfForSource<TEntity>(bool condition, Expression<Func<TEntity, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            if (!condition)
                return;

            AddForSource(predicate, logicalOperator);
        }

        /// <summary>
        /// Adds an IN or NOT IN collection condition for an entity available
        /// in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The entity type associated with the selected property.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The selected property and collection element type.
        /// </typeparam>
        /// <param name="selector">
        /// The expression that selects the property evaluated by the collection condition.
        /// </param>
        /// <param name="values">
        /// The values evaluated by the collection condition.
        /// </param>
        /// <param name="isNegated">
        /// A value indicating whether the collection condition uses NOT IN.
        /// </param>
        public void AddCollection<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> selector, IEnumerable<TProperty> values, bool isNegated)
        {
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentNullException.ThrowIfNull(values);

            var materializedValues = new List<object>();

            foreach (var value in values)
            {
                if (value is null)
                    throw new ArgumentException("IN and NOT IN collections cannot contain null values.", nameof(values));

                materializedValues.Add(value);
            }

            if (materializedValues.Count == 0)
                throw new ArgumentException("IN and NOT IN collections must contain at least one value.", nameof(values));

            var sourceDefinition = _sourceResolver.Resolve<TEntity>();

            _context.QueryDefinition
                .WhereCollectionDefinitions
                .Add(
                    new QueryWhereCollectionDefinition
                    {
                        Selector = selector,
                        Source = sourceDefinition,
                        Values = materializedValues,
                        IsNegated = isNegated
                    });
        }

        /// <summary>
        /// Adds a scalar SQL function WHERE condition.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The entity type associated with the selected column.
        /// </typeparam>
        /// <param name="function">
        /// The scalar SQL function to apply.
        /// </param>
        /// <param name="selector">
        /// The expression that selects the target entity property.
        /// </param>
        /// <param name="comparisonOperator">
        /// The comparison operator applied to the scalar function result.
        /// </param>
        /// <param name="value">
        /// The value compared with the scalar function result.
        /// </param>
        public void AddFunction<TEntity>(
            QueryScalarFunction function,
            Expression<Func<TEntity, object>> selector,
            QueryComparisonOperator comparisonOperator,
            object? value)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var sourceDefinition =
                _sourceResolver.Resolve<TEntity>();

            var propertyName = QueryColumnExpressionExtractor
                .ExtractColumns(selector)
                .Single()
                .PropertyName;

            _context.QueryDefinition
                .WhereScalarFunctionDefinitions
                .Add(
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
        /// <typeparam name="TEntity">
        /// The entity type associated with the computed expression.
        /// </typeparam>
        /// <param name="expression">
        /// The computed predicate expression to add.
        /// </param>
        public void AddComputed<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var sourceDefinition =_sourceResolver.Resolve<TEntity>();

            _context.QueryDefinition
                .WhereComputedExpressionDefinitions
                .Add(
                    new QueryWhereComputedExpressionDefinition
                    {
                        Expression = expression,
                        Sources =
                            new Dictionary<ParameterExpression,QuerySourceDefinition>
                            {
                                [expression.Parameters[0]] = sourceDefinition
                            }
                    });
        }

        /// <summary>
        /// Adds a computed WHERE expression for two entity sources.
        /// </summary>
        /// <typeparam name="TLeft">
        /// The left entity type associated with the computed expression.
        /// </typeparam>
        /// <typeparam name="TRight">
        /// The right entity type associated with the computed expression.
        /// </typeparam>
        /// <param name="expression">
        /// The computed predicate expression to add.
        /// </param>
        public void AddComputed<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var leftSource =
                _sourceResolver.Resolve<TLeft>();

            var rightSource =
                _sourceResolver.Resolve<TRight>();

            _context.QueryDefinition
                .WhereComputedExpressionDefinitions
                .Add(
                    new QueryWhereComputedExpressionDefinition
                    {
                        Expression = expression,
                        Sources = new Dictionary<ParameterExpression,QuerySourceDefinition>
                            {
                                [expression.Parameters[0]] = leftSource,
                                [expression.Parameters[1]] = rightSource
                            }
                    });
        }

        /// <summary>
        /// Adds a predicate definition to the current query using the
        /// resolved source and logical operator.
        /// </summary>
        /// <param name="predicate">
        /// The predicate expression associated with the condition.
        /// </param>
        /// <param name="sourceDefinition">
        /// The query source associated with the predicate.
        /// </param>
        /// <param name="logicalOperator">
        /// The logical operator used to connect the predicate with
        /// the preceding predicate.
        /// </param>
        private void AddInternal(LambdaExpression predicate, QuerySourceDefinition sourceDefinition, QueryLogicalOperator logicalOperator)
        {
            _context.QueryDefinition.WhereDefinitions.Add(
                new QueryWhereDefinition
                {
                    PredicateExpression = predicate,
                    Source = sourceDefinition,
                    LogicalOperator = logicalOperator
                });
        }

    }
}

