using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.ExpressionScopes;
using TinyBlueWhale.EngineQuery.Core.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Helpers
{
    /// <summary>
    /// Resolves SQL column references from scoped expression parameters.
    /// </summary>
    public sealed class ScopedColumnReferenceResolver(ISqlDatabaseDialect databaseDialect, QueryExpressionScope expressionScope)
    {
        private readonly ISqlDatabaseDialect _databaseDialect = databaseDialect;
        private readonly QueryExpressionScope _expressionScope = expressionScope;

        /// <summary>
        /// Resolves a SQL column reference from a scoped member expression.
        /// </summary>
        /// <param name="memberExpression">
        /// Member expression associated with a query source property.
        /// </param>
        /// <returns>
        /// Escaped SQL column reference, qualified with the source alias when available.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="memberExpression"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when the specified expression does not represent a scoped query source property.
        /// </exception>
        public string Resolve(MemberExpression memberExpression)
        {
            ArgumentNullException.ThrowIfNull(memberExpression);

            if (memberExpression.Expression is not ParameterExpression parameterExpression)
                throw new NotSupportedException($"Expression '{memberExpression}' is not a scoped column reference.");

            var source = _expressionScope.Resolve(parameterExpression);

            var propertyName = memberExpression.Member.Name;

            var columnName = source.ColumnMappings.TryGetValue(
                propertyName,
                out var mappedColumnName)
                    ? mappedColumnName
                    : propertyName;

            return string.IsNullOrWhiteSpace(source.TableAlias)
                ? _databaseDialect.EscapeIdentifier(columnName)
                : _databaseDialect.BuildQualifiedIdentifier(source.TableAlias, columnName);
        }
    }
}
