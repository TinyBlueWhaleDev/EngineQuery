using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    public interface IOrderedQueryCommandBuilder<T> : IQueryCommandBuilder<T>
    {
        IOrderedQueryCommandBuilder<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector);

        IOrderedQueryCommandBuilder<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector);
    }
}
