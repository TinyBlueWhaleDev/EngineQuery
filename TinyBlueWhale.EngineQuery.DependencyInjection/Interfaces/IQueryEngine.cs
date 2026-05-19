using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;

namespace TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces
{
    /// <summary>
    /// Represents a configured EngineQuery query builder.
    /// </summary>
    public interface IQueryEngine : IQueryBuilder
    {
    }
}
