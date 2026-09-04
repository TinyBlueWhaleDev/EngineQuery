using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.Parameters;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Formatting;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Compilation
{
    /// <summary>
    /// Provides a shared base implementation for SQL query compilers.
    /// </summary>
    /// <remarks>
    /// This compiler transforms compiled query definitions into provider-specific SQL command text
    /// while delegating SQL script construction, capability validation and dialect-specific syntax
    /// to dedicated collaborators.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="QueryCompilerBase"/> class.
    /// </remarks>
    /// <param name="databaseDialect">
    /// SQL database dialect used to escape identifiers and build provider-specific SQL fragments.
    /// </param>
    /// <param name="providerCapabilities">
    /// Provider capability definition used to validate whether the current query can be compiled.
    /// </param>
    /// <param name="queryScriptBuilder">
    /// SQL script builder used to generate unformatted SQL command text.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="databaseDialect"/>, <paramref name="providerCapabilities"/> or
    /// <paramref name="queryScriptBuilder"/> is <see langword="null"/>.
    /// </exception>
    public abstract class QueryCompilerBase(ISqlDatabaseDialect databaseDialect, IQueryScriptBuilder queryScriptBuilder) : IQueryCompiler
    {
        /// <summary>
        /// SQL database dialect used to escape identifiers and build provider-specific SQL fragments.
        /// </summary>
        protected readonly ISqlDatabaseDialect _databaseDialect = databaseDialect ?? throw new ArgumentNullException(nameof(databaseDialect));

        private readonly IQueryScriptBuilder _queryScriptBuilder = queryScriptBuilder ?? throw new ArgumentNullException(nameof(queryScriptBuilder));

        /// <summary>
        /// Compiles the specified query definition into a generated SQL query.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to compile.
        /// </param>
        /// <returns>
        /// Generated SQL query containing formatted command text and SQL parameters.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryDefinition"/> is <see langword="null"/>.
        /// </exception>
        public GeneratedSqlQuery Compile(CompiledQueryDefinition queryDefinition)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);

            var context = new QueryCompilationContext(_databaseDialect, new QueryParameterCollection());

            var commandText = _queryScriptBuilder.Build(queryDefinition, context);

            var formattedCommandText = SqlScriptFormatter.Format(commandText);

            return new GeneratedSqlQuery
            {
                CommandText = formattedCommandText,
                Parameters = context.Parameters.Parameters
            };
        }
    }
}
