using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Sql.Compilation.Models
{
    public sealed record QueryWhereDefinition()
    {
        public LambdaExpression PredicateExpression { get; init; } = null!; 
    }
}
