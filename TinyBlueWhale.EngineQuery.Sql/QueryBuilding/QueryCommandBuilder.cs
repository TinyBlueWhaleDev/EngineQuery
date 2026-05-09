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
    public sealed class QueryCommandBuilder<T> : IOrderedQueryCommandBuilder<T>
    {
        private readonly ISqlDatabaseDialect _databaseDialect;
        private readonly CompiledQueryDefinition _queryDefinition;

        public QueryCommandBuilder(ISqlDatabaseDialect databaseDialect)
        {
            _databaseDialect = databaseDialect;

            _queryDefinition = new CompiledQueryDefinition
            {
                TableName = typeof(T).Name + "s"
            };
        }

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

        public IQueryCommandBuilder<T> WhereIf(
            bool condition,
            Expression<Func<T, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            return condition
                ? Where(predicate)
                : this;
        }

        public IOrderedQueryCommandBuilder<T> OrderBy<TKey>(
            Expression<Func<T, TKey>> keySelector)
        {
            ArgumentNullException.ThrowIfNull(keySelector);

            AddOrderingDefinition(
                keySelector,
                QueryOrderingDirection.Ascending);

            return this;
        }

        public IOrderedQueryCommandBuilder<T> OrderByDescending<TKey>(
            Expression<Func<T, TKey>> keySelector)
        {
            ArgumentNullException.ThrowIfNull(keySelector);

            AddOrderingDefinition(
                keySelector,
                QueryOrderingDirection.Descending);

            return this;
        }

        public IOrderedQueryCommandBuilder<T> ThenBy<TKey>(
            Expression<Func<T, TKey>> keySelector)
        {
            ArgumentNullException.ThrowIfNull(keySelector);

            AddOrderingDefinition(
                keySelector,
                QueryOrderingDirection.Ascending);

            return this;
        }

        public IOrderedQueryCommandBuilder<T> ThenByDescending<TKey>(
            Expression<Func<T, TKey>> keySelector)
        {
            ArgumentNullException.ThrowIfNull(keySelector);

            AddOrderingDefinition(
                keySelector,
                QueryOrderingDirection.Descending);

            return this;
        }

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

        public GeneratedSqlQuery ToSql()
        {
            var compiler = new QuerySqlCompiler(_databaseDialect);

            return compiler.CompileToSql(_queryDefinition);
        }

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

        private static IReadOnlyList<string> ExtractSelectedProperties(Expression<Func<T, object>> selector)
        {
            return SelectedPropertyExpressionExtractor.ExtractSelectedProperties(selector);
        }
    }
}
