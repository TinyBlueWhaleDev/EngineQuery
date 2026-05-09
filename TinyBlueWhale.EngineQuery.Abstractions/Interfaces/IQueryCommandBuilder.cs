using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    public interface IQueryCommandBuilder<T>
    {
        IQueryCommandBuilder<T> Select(Expression<Func<T, object>> selector);
        IQueryCommandBuilder<T> Where(Expression<Func<T, bool>> predicate);
        IQueryCommandBuilder<T> WhereIf(bool condition, Expression<Func<T, bool>> predicate);
        IOrderedQueryCommandBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector);
        IOrderedQueryCommandBuilder<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector);
        IQueryCommandBuilder<T> Skip(int count);
        IQueryCommandBuilder<T> Take(int count);
        GeneratedSqlQuery ToSql();
    }
}
