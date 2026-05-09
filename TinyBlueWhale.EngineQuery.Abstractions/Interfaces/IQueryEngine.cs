using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    public interface IQueryEngine
    {
        IQueryCommandBuilder<T> Query<T>();
    }
}
