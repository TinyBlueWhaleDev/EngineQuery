using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Sql.Compilation.Models
{
    /// <summary>
    /// Represents a filtering definition used to generate SQL WHERE conditions.
    /// </summary>
    public sealed record QueryWhereDefinition()
    {
        /// <summary>
        /// Gets the predicate expression associated with the filter definition.
        /// </summary>
        public LambdaExpression PredicateExpression { get; init; } = null!; 
    }
}
