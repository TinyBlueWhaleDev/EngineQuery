using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Sql.ExpressionParsing
{
    public static class RuntimeExpressionValueExtractor
    {
        public static object? ExtractValue(Expression expression)
        {
            var lambda = Expression.Lambda(expression);
            var compiledExpression = lambda.Compile();

            return compiledExpression.DynamicInvoke();
        }
    }
}
