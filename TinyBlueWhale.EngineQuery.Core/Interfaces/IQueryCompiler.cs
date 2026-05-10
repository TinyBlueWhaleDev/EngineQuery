using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.Core.Interfaces
{
    /// <summary>
    /// Defines a contract for compiling query definitions into generated query output.
    /// </summary>
    public interface IQueryCompiler
    {
        /// <summary>
        /// Compiles the specified query definition into a generated query.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition containing the intent to compile.
        /// </param>
        /// <returns>
        /// Generated SQL query output.
        /// </returns>
        GeneratedSqlQuery Compile(CompiledQueryDefinition queryDefinition);
    }
}
