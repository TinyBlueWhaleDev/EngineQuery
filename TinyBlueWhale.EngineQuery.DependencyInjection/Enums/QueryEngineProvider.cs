using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.DependencyInjection.Enums
{
    /// <summary>
    /// Defines the supported EngineQuery providers.
    /// </summary>
    public enum QueryEngineProvider
    {
        SqlServer = 1,
        MySql = 2,
        PostgreSql = 3
    }
}
