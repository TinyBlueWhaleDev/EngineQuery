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
        /// <summary>
        /// Represents the Microsoft SQL Server provider.
        /// </summary>
        SqlServer = 1,

        /// <summary>
        /// Represents the MySQL database provider.
        /// </summary>
        MySql = 2,

        /// <summary>
        /// Represents the PostgreSQL database provider.
        /// </summary>
        PostgreSql = 3
    }
}
