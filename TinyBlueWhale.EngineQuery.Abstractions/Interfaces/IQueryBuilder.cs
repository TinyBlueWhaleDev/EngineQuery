namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Represents the main entry point for creating strongly typed query builders.
    /// </summary>
    public interface IQueryBuilder
    {
        /// <summary>
        /// Creates a new query command builder for the specified entity type and table name.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type used as the source of the query.
        /// </typeparam>
        /// <param name="tableName">      
        /// Database table name associated with the query.
        /// </param>
        /// <param name="alias">
        /// Optional table alias used to qualify generated SQL column references.
        /// </param>
        /// <returns>
        /// A fluent query command builder for composing and generating SQL queries.
        /// </returns>
        IQueryCommandBuilder<T> From<T>(string tableName, string? alias = null);

        /// <summary>
        /// Creates a new query builder using resolved entity metadata.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type used as the source of the query.
        /// </typeparam>
        /// /// <param name="alias">
        /// Optional table alias used to qualify generated SQL column references.
        /// </param>
        /// <returns>
        /// Fluent query command builder.
        /// </returns>
        IQueryCommandBuilder<T> From<T>(string? alias = null);

        /// <summary>
        /// Creates a query command builder using a derived table as the root query source.
        /// </summary>
        /// <typeparam name="TDerived">
        /// CLR type used to represent the derived table projection.
        /// </typeparam>
        /// <typeparam name="TSubqueryRoot">
        /// Root entity type used by the derived table subquery.
        /// </typeparam>
        /// <param name="alias">
        /// Alias assigned to the derived table.
        /// </param>
        /// <param name="subqueryBuilder">
        /// Function used to build the derived table subquery.
        /// </param>
        /// <returns>
        /// Query command builder for the derived table source.
        /// </returns>
        IQueryCommandBuilder<TDerived> FromSubquery<TDerived, TSubqueryRoot>(string alias, Func<IQueryBuilder, IQueryCommandBuilder<TSubqueryRoot>> subqueryBuilder);
    }
}
