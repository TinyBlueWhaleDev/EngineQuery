namespace TinyBlueWhale.EngineQuery.Tests.Providers.MySql
{
    ///// <summary>
    ///// Provides MySQL-specific query builder infrastructure
    ///// for shared query feature tests.
    ///// </summary>
    //internal sealed class MySqlQueryTestProvider : IQueryTestProvider
    //{
    //    /// <summary>
    //    /// Gets the provider name used by shared test infrastructure.
    //    /// </summary>
    //    public string ProviderName => "MySql";

    //    /// <summary>
    //    /// Creates a query builder configured for MySQL.
    //    /// </summary>
    //    /// <returns>
    //    /// Query builder configured with MySQL compilation components
    //    /// and the shared test metadata resolver.
    //    /// </returns>
    //    public QueryBuilder CreateQueryBuilder()
    //    {
    //        return new QueryBuilder(
    //            new MySqlQueryCompiler(
    //                new MySqlDatabaseDialect(),
    //                new MySqlProviderCapabilities()),
    //            TestMetadataFactory.CreateMetadataResolver());
    //    }

    //    /// <summary>
    //    /// Creates a query builder configured with the specified
    //    /// MySQL provider capabilities.
    //    /// </summary>
    //    /// <param name="capabilities">
    //    /// Provider capabilities used by the MySQL query compiler.
    //    /// </param>
    //    /// <returns>
    //    /// Query builder configured with the supplied capabilities.
    //    /// </returns>
    //    public QueryBuilder CreateQueryBuilder(IDatabaseProviderCapabilities capabilities)
    //    {
    //        ArgumentNullException.ThrowIfNull(capabilities);

    //        return new QueryBuilder(
    //            new MySqlQueryCompiler(
    //                new MySqlDatabaseDialect(),
    //                capabilities),
    //            TestMetadataFactory.CreateMetadataResolver());
    //    }

    //    /// <summary>
    //    /// Returns the provider name for readable NUnit fixture output.
    //    /// </summary>
    //    /// <returns>
    //    /// MySQL provider name.
    //    /// </returns>
    //    public override string ToString()
    //    {
    //        return ProviderName;
    //    }
    //}
}
