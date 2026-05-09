using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Compilation.Models;
using TinyBlueWhale.EngineQuery.Sql.Dialects.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Enums;
using TinyBlueWhale.EngineQuery.Sql.ExpressionParsing;

namespace TinyBlueWhale.EngineQuery.Sql.QueryBuilding
{
    /// <summary>
    /// Builds strongly typed query definitions using a fluent API.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type used as the source of the query.
    /// </typeparam>
    /// <remarks>
    /// This builder does not execute database commands.
    /// It only captures query intent and delegates SQL generation to the query compiler.
    /// </remarks>
    public sealed class QueryCommandBuilder<T> : IOrderedQueryCommandBuilder<T>
    {
        private readonly ISqlDatabaseDialect _databaseDialect;
        private readonly CompiledQueryDefinition _queryDefinition;

        /// <summary>
        /// Adds selected entity properties to the query projection definition.
        /// </summary>
        /// <param name="selector">
        /// Projection expression that determines which properties are included in the SQL SELECT clause.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="selector"/> is null.
        /// </exception>
        public IQueryCommandBuilder<T> Select(Expression<Func<T, object>> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var selectedProperties = ExtractSelectedProperties(selector);

            foreach (var propertyName in selectedProperties)
            {
                _queryDefinition.SelectDefinitions.Add(
                    new QuerySelectColumnDefinition
                    {
                        PropertyName = propertyName
                    });
            }

            return this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommandBuilder{T}"/> class.
        /// </summary>
        /// <param name="databaseDialect">
        /// SQL database dialect used to generate provider-specific SQL syntax.
        /// </param>
        public QueryCommandBuilder(ISqlDatabaseDialect databaseDialect)
        {
            _databaseDialect = databaseDialect;

            _queryDefinition = new CompiledQueryDefinition
            {
                TableName = typeof(T).Name + "s"
            };
        }

        /// <summary>
        /// Adds a filtering expression to the query definition.
        /// </summary>
        /// <param name="predicate">
        /// Predicate expression used later to generate the SQL WHERE clause.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="predicate"/> is null.
        /// </exception>
        public IQueryCommandBuilder<T> Where(
            Expression<Func<T, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            _queryDefinition.WhereDefinitions.Add(new QueryWhereDefinition
            {
                PredicateExpression = predicate
            });

            return this;
        }

        /// <summary>
        /// Adds a filtering expression only when the specified condition is true.
        /// </summary>
        /// <param name="condition">
        /// Determines whether the predicate should be added to the query definition.
        /// </param>
        /// <param name="predicate">
        /// Predicate expression used later to generate the SQL WHERE clause.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="predicate"/> is null.
        /// </exception>
        public IQueryCommandBuilder<T> WhereIf(
            bool condition,
            Expression<Func<T, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            return condition
                ? Where(predicate)
                : this;
        }

        /// <summary>
        /// Adds an ascending ordering expression to the query definition.
        /// </summary>
        /// <typeparam name="TKey">
        /// Type of the selected ordering property.
        /// </typeparam>
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Ordered query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="keySelector"/> is null.
        /// </exception>
        public IOrderedQueryCommandBuilder<T> OrderBy<TKey>(
            Expression<Func<T, TKey>> keySelector)
        {
            ArgumentNullException.ThrowIfNull(keySelector);

            AddOrderingDefinition(
                keySelector,
                QueryOrderingDirection.Ascending);

            return this;
        }

        /// <summary>
        /// Adds a descending ordering expression to the query definition.
        /// </summary>
        /// <typeparam name="TKey">
        /// Type of the selected ordering property.
        /// </typeparam>
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Ordered query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="keySelector"/> is null.
        /// </exception>
        public IOrderedQueryCommandBuilder<T> OrderByDescending<TKey>(
            Expression<Func<T, TKey>> keySelector)
        {
            ArgumentNullException.ThrowIfNull(keySelector);

            AddOrderingDefinition(
                keySelector,
                QueryOrderingDirection.Descending);

            return this;
        }

        /// <summary>
        /// Adds an additional ascending ordering expression to the query definition.
        /// </summary>
        public IOrderedQueryCommandBuilder<T> ThenBy<TKey>(
            Expression<Func<T, TKey>> keySelector)
        {
            ArgumentNullException.ThrowIfNull(keySelector);

            AddOrderingDefinition(
                keySelector,
                QueryOrderingDirection.Ascending);

            return this;
        }

        /// <summary>
        /// Adds an additional descending ordering expression to the query definition.
        /// </summary>
        public IOrderedQueryCommandBuilder<T> ThenByDescending<TKey>(
            Expression<Func<T, TKey>> keySelector)
        {
            ArgumentNullException.ThrowIfNull(keySelector);

            AddOrderingDefinition(
                keySelector,
                QueryOrderingDirection.Descending);

            return this;
        }

        /// <summary>
        /// Sets the number of rows to skip during SQL pagination.
        /// </summary>
        /// <param name="count">
        /// Number of rows to skip.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="count"/> is negative.
        /// </exception>
        public IQueryCommandBuilder<T> Skip(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    "Skip count cannot be negative.");
            }

            _queryDefinition.Pagination =
                _queryDefinition.Pagination with
                {
                    Skip = count
                };

            return this;
        }

        /// <summary>
        /// Sets the maximum number of rows returned during SQL pagination.
        /// </summary>
        /// <param name="count">
        /// Maximum number of rows to return.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="count"/> is less than or equal to zero.
        /// </exception>
        public IQueryCommandBuilder<T> Take(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    "Take count must be greater than zero.");
            }

            _queryDefinition.Pagination =
                _queryDefinition.Pagination with
                {
                    Take = count
                };

            return this;
        }

        /// <summary>
        /// Compiles the current query definition into SQL command text and parameters.
        /// </summary>
        /// <returns>
        /// Generated SQL query command.
        /// </returns>
        public GeneratedSqlQuery ToSql()
        {
            var compiler = new QuerySqlCompiler(_databaseDialect);

            return compiler.CompileToSql(_queryDefinition);
        }

        // Registers an ordering definition preserving the fluent ordering sequence.
        private void AddOrderingDefinition<TKey>(
            Expression<Func<T, TKey>> keySelector,
            QueryOrderingDirection orderingDirection)
        {
            var propertyName = ExtractPropertyNameFromExpression(keySelector);

            _queryDefinition.OrderingDefinitions.Add(
                new QueryOrderingDefinition
                {
                    PropertyName = propertyName,
                    Direction = orderingDirection
                });
        }

        // Extracts property names from direct or converted member access expressions.
        private static string ExtractPropertyNameFromExpression<TKey>(
            Expression<Func<T, TKey>> expression)
        {
            return expression.Body switch
            {
                MemberExpression memberExpression =>
                    memberExpression.Member.Name,

                UnaryExpression unaryExpression
                    when unaryExpression.Operand is MemberExpression memberExpression =>
                    memberExpression.Member.Name,

                _ => throw new NotSupportedException(
                    $"Expression '{expression}' is not supported as an ordering selector.")
            };
        }

        private static IReadOnlyList<string> ExtractSelectedProperties(Expression<Func<T, object>> selector)
        {
            return SelectedPropertyExpressionExtractor.ExtractSelectedProperties(selector);
        }
    }
}
