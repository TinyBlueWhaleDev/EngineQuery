using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Represents the main entry point for creating strongly typed query builders.
    /// </summary>
    public interface IQueryBuilder<TProfile>
        where TProfile : IDatabaseProviderProfile
    {

        /// <summary>
        /// Creates a new query builder using resolved entity metadata.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type used as the source of the query.
        /// </typeparam>        
        /// <returns>
        /// Fluent query command builder.
        /// </returns>
        IQueryCommandBuilder<T, TProfile> From<T>();

        /// <summary>
        /// Creates a new query builder using resolved entity metadata.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type used as the source of the query.
        /// </typeparam>
        /// <param name="alias">
        /// Optional table alias used to qualify generated SQL column references.
        /// </param>
        /// <returns>
        /// Fluent query command builder.
        /// </returns>
        IQueryCommandBuilder<T, TProfile> From<T>(string alias);

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
        IQueryCommandBuilder<T, TProfile> From<T>(string tableName, string alias);

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
        IQueryCommandBuilder<TDerived, TProfile> FromSubquery<TDerived, TSubqueryRoot>(string alias, Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSubqueryRoot, TProfile>> subqueryBuilder);

        /// <summary>
        /// Creates a new INSERT command builder for the specified entity type and table name.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the target INSERT table.
        /// </typeparam>
        /// <param name="tableName">
        /// Database table name associated with the INSERT command.
        /// </param>
        /// <returns>
        /// Fluent INSERT command builder.
        /// </returns>
        IInsertCommandBuilder<T, TProfile> InsertInto<T>(string tableName);

        /// <summary>
        /// Creates a new INSERT command builder using resolved entity metadata.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the target INSERT table.
        /// </typeparam>
        /// <returns>
        /// Fluent INSERT command builder.
        /// </returns>
        IInsertCommandBuilder<T, TProfile> InsertInto<T>();

        /// <summary>
        /// Creates a new UPDATE command builder for the specified entity type and table name.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the target UPDATE table.
        /// </typeparam>
        /// <param name="tableName">
        /// Database table name associated with the UPDATE command.
        /// </param>
        /// <returns>
        /// Fluent UPDATE command builder.
        /// </returns>
        IUpdateCommandBuilder<T> Update<T>(string tableName);

        /// <summary>
        /// Creates a new UPDATE command builder using resolved entity metadata.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the target UPDATE table.
        /// </typeparam>
        /// <returns>
        /// Fluent UPDATE command builder.
        /// </returns>
        IUpdateCommandBuilder<T> Update<T>();

        /// <summary>
        /// Creates a new DELETE command builder for the specified entity type and table name.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the target DELETE table.
        /// </typeparam>
        /// <param name="tableName">
        /// Database table name associated with the DELETE command.
        /// </param>
        /// <returns>
        /// Fluent DELETE command builder.
        /// </returns>
        IDeleteCommandBuilder<T> DeleteFrom<T>(string tableName);

        /// <summary>
        /// Creates a new DELETE command builder using resolved entity metadata.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the target DELETE table.
        /// </typeparam>
        /// <returns>
        /// Fluent DELETE command builder.
        /// </returns>
        IDeleteCommandBuilder<T> DeleteFrom<T>();

        /// <summary>
        /// Registers an internal common table expression definition.
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
        /// Current query builder instance.
        /// </returns>
        internal IQueryBuilder<TProfile> With<TCte, TSubqueryRoot>(string name, Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSubqueryRoot, TProfile>> cteBuilder)
        {
            throw new NotSupportedException("Common table expression registration is not supported by the current query builder.");
        }

        /// <summary>
        /// Creates an internal query command builder using a common table expression as the root source.
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
        internal IQueryCommandBuilder<TCte, TProfile> FromCte<TCte>(string name, string? alias = null)
        {
            throw new NotSupportedException("Common table expression sources are not supported by the current query builder.");
        }

        /// <summary>
        /// Registers an internal recursive common table expression definition.
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
        /// Current query builder instance.
        /// </returns>
        internal IQueryBuilder<TProfile> WithRecursive<TCte, TBaseRoot, TRecursiveRoot>(string name, Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TBaseRoot, TProfile>> baseQueryBuilder, Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TRecursiveRoot, TProfile>> recursiveQueryBuilder)
        {
            throw new NotSupportedException("Recursive common table expression registration is not supported by the current query builder.");
        }
    }
}
