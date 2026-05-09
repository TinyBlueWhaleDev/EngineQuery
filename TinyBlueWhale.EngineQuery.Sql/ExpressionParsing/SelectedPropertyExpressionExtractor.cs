using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Sql.ExpressionParsing
{
    public static class SelectedPropertyExpressionExtractor
    {
        public static IReadOnlyList<string> ExtractSelectedProperties<T>(
            Expression<Func<T, object>> selector)
        {
            return selector.Body switch
            {
                NewExpression newExpression => ExtractFromNewExpression(newExpression),

                MemberExpression memberExpression => [memberExpression.Member.Name],

                UnaryExpression unaryExpression when unaryExpression.Operand is MemberExpression memberExpression => [memberExpression.Member.Name],

                _ => throw new NotSupportedException($"Select expression '{selector}' is not supported.")
            };
        }

        private static IReadOnlyList<string> ExtractFromNewExpression(NewExpression newExpression)
        {
            return [.. newExpression.Arguments.OfType<MemberExpression>().Select(x => x.Member.Name)];
        }
    }
}
