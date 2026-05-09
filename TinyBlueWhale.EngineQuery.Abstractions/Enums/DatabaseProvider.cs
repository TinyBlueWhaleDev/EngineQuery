using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Enums
{
    /// <summary>
    /// Represents the supported database providers available for query compilation and execution.
    /// </summary>
    public enum DatabaseProvider
    {
        /// <summary>
        /// Microsoft SQL Server provider.
        /// </summary>
        SqlServer = 1,

        /// <summary>
        /// PostgreSQL provider.
        /// </summary>
        PostgreSql = 2,

        /// <summary>
        /// MySQL provider.
        /// </summary>
        MySql = 3
    }
}
