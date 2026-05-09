using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    public interface IQueryEngineFactory
    {
        IQueryEngine For(DatabaseProvider provider);
    }
}
