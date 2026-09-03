using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features
{
    /// <summary>
    /// Defines root query operations available for provider profiles that support common table expressions.
    /// </summary>
    /// <typeparam name="TProfile">
    /// Database provider profile type.
    /// </typeparam>
    public interface ICTEQueryBuilder<TProfile> : IQueryBuilder<TProfile>
        where TProfile : IDatabaseProviderProfile, ICTEFeature
    {
        /// <summary>
        /// Registers a common table expression that can be used as a query source.
        /// </summary>
        /// <typeparam name="TCte">
        /// CLR type used to represent the common table expression projection.
        /// </typeparam>
        /// <typeparam name="TSubqueryRoot">
        /// Root entity type used by the common table expression query.
        /// </typeparam>
        /// <param name="name">
        /// Common table expression name.
        /// </param>
        /// <param name="cteBuilder">
        /// Function used to build the common table expression query.
        /// </param>
        /// <returns>
        /// Current CTE query builder surface.
        /// </returns>
        new ICTEQueryBuilder<TProfile> With<TCte, TSubqueryRoot>(string name, Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSubqueryRoot, TProfile>> cteBuilder);

        /// <summary>
        /// Creates a query command builder using a common table expression as the root source.
        /// </summary>
        /// <typeparam name="TCte">
        /// CLR type used to represent the common table expression projection.
        /// </typeparam>
        /// <param name="name">
        /// Common table expression name.
        /// </param>
        /// <param name="alias">
        /// Optional alias assigned to the common table expression source.
        /// </param>
        /// <returns>
        /// Query command builder for the common table expression source.
        /// </returns>
        new IQueryCommandBuilder<TCte, TProfile> FromCte<TCte>(string name, string? alias = null);
    }

    /// <summary>
    /// Defines root query operations available for provider profiles that support recursive common table expressions.
    /// </summary>
    /// <typeparam name="TProfile">
    /// Database provider profile type.
    /// </typeparam>
    public interface IRecursiveCTEQueryBuilder<TProfile> : ICTEQueryBuilder<TProfile>
        where TProfile : IDatabaseProviderProfile, IRecursiveCTEFeature
    {
        /// <summary>
        /// Registers a common table expression while preserving the recursive CTE query builder surface.
        /// </summary>
        /// <typeparam name="TCte">
        /// CLR type used to represent the common table expression projection.
        /// </typeparam>
        /// <typeparam name="TSubqueryRoot">
        /// Root entity type used by the common table expression query.
        /// </typeparam>
        /// <param name="name">
        /// Common table expression name.
        /// </param>
        /// <param name="cteBuilder">
        /// Function used to build the common table expression query.
        /// </param>
        /// <returns>
        /// Current recursive CTE query builder surface.
        /// </returns>
        new IRecursiveCTEQueryBuilder<TProfile> With<TCte, TSubqueryRoot>(string name, Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSubqueryRoot, TProfile>> cteBuilder);

        /// <summary>
        /// Registers a recursive common table expression that can be used as a query source.
        /// </summary>
        /// <typeparam name="TCte">
        /// CLR type used to represent the recursive common table expression projection.
        /// </typeparam>
        /// <typeparam name="TBaseRoot">
        /// Root entity type used by the recursive common table expression base query.
        /// </typeparam>
        /// <typeparam name="TRecursiveRoot">
        /// Root entity type used by the recursive common table expression recursive query.
        /// </typeparam>
        /// <param name="name">
        /// Common table expression name.
        /// </param>
        /// <param name="baseQueryBuilder">
        /// Function used to build the base query.
        /// </param>
        /// <param name="recursiveQueryBuilder">
        /// Function used to build the recursive query.
        /// </param>
        /// <returns>
        /// Current recursive CTE query builder surface.
        /// </returns>
        new IRecursiveCTEQueryBuilder<TProfile> WithRecursive<TCte, TBaseRoot, TRecursiveRoot>(string name, Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TBaseRoot, TProfile>> baseQueryBuilder, Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TRecursiveRoot, TProfile>> recursiveQueryBuilder);
    }
}
