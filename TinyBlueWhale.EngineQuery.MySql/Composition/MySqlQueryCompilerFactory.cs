using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.MySql.Clauses;
using TinyBlueWhale.EngineQuery.Sql.Composition;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.MySql.Composition
{
    /// <summary>
    /// Creates MySQL query compiler collaborators.
    /// </summary>
    /// <remarks>
    /// This factory configures only the MySQL-specific clause builders while reusing
    /// the shared SQL query compilation pipeline from the base project.
    /// </remarks>
    public static class MySqlQueryCompilerFactory
    {
        /// <summary>
        /// Creates the MySQL query script builder.
        /// </summary>
        /// <param name="databaseDialect">
        /// MySQL database dialect.
        /// </param>
        /// <param name="featureComposition">
        /// SQL feature composition resolved from the selected provider profile.
        /// </param>
        /// <returns>
        /// Configured MySQL query script builder.
        /// </returns>
        public static IQueryScriptBuilder CreateScriptBuilder(
            ISqlDatabaseDialect databaseDialect,
            QueryFeatureComposition featureComposition)
        {
            ArgumentNullException.ThrowIfNull(databaseDialect);
            ArgumentNullException.ThrowIfNull(featureComposition);

            return QueryCompilerFactory.CreateScriptBuilder(
                databaseDialect,
                new QueryScriptBuilderOptions
                {
                    InsertIdentityRetrievalStrategy = featureComposition.InsertIdentityRetrievalStrategy,
                    PaginationStrategy = featureComposition.PaginationStrategy,
                    CteStrategy = featureComposition.CteStrategy,
                    LateralJoinStrategy = featureComposition.LateralJoinStrategy
                });
        }
    }
}
