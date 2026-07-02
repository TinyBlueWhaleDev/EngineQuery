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
        /// Resolves a SQL column reference from a member expression.
        /// </summary>
        public string Resolve(MemberExpression memberExpression)
        {
            ArgumentNullException.ThrowIfNull(memberExpression);

            if (memberExpression.Expression is not ParameterExpression parameterExpression)
                throw new NotSupportedException($"Expression '{memberExpression}' is not a scoped column reference.");

            var source = _expressionScope.Resolve(parameterExpression);
            var propertyName = memberExpression.Member.Name;

            var columnName = source.ColumnMappings.TryGetValue(propertyName, out var mappedColumnName)
                ? mappedColumnName
                : propertyName;

            return _databaseDialect.BuildQualifiedIdentifier(source.TableAlias, columnName);
        }
    }
}
