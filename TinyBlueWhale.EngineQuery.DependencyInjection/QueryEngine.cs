using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces;

namespace TinyBlueWhale.EngineQuery.DependencyInjection
{
    /// <summary>
    /// Default configured EngineQuery implementation associated with a database provider profile.
    /// </summary>
    /// <typeparam name="TProfile">
    /// Database provider profile associated with the query engine.
    /// </typeparam>
    internal class QueryEngine<TProfile>(QueryBuilder<TProfile> innerQueryBuilder) :
        IQueryEngine<TProfile>
        where TProfile : IDatabaseProviderProfile
    {
        protected readonly QueryBuilder<TProfile> _innerQueryBuilder = innerQueryBuilder ?? throw new ArgumentNullException(nameof(innerQueryBuilder));

        /// <inheritdoc />
        public IQueryCommandBuilder<T, TProfile> From<T>()
        {
            return _innerQueryBuilder.From<T>();
        }

        /// <inheritdoc />
        public IQueryCommandBuilder<T, TProfile> From<T>(string alias)
        {
            return _innerQueryBuilder.From<T>(alias);
        }

        /// <inheritdoc />
        public IQueryCommandBuilder<T, TProfile> From<T>(string tableName, string alias)
        {
            return _innerQueryBuilder.From<T>(tableName, alias);
        }

        /// <inheritdoc />
        public IQueryCommandBuilder<TDerived, TProfile> FromSubquery<TDerived, TSubqueryRoot>(
            string alias,
            Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSubqueryRoot, TProfile>> subqueryBuilder)
        {
            return _innerQueryBuilder.FromSubquery<TDerived, TSubqueryRoot>(alias, subqueryBuilder);
        }

        /// <inheritdoc />
        public IQueryBuilder<TProfile> With<TCte, TSubqueryRoot>(
            string name,
            Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSubqueryRoot, TProfile>> cteBuilder)
        {
            _innerQueryBuilder.With<TCte, TSubqueryRoot>(name, cteBuilder);
            return this;
        }

        /// <inheritdoc />
        public IQueryCommandBuilder<TCte, TProfile> FromCte<TCte>(string name, string? alias = null)
        {
            return _innerQueryBuilder.FromCte<TCte>(name, alias);
        }

        /// <inheritdoc />
        public IQueryBuilder<TProfile> WithRecursive<TCte, TBaseRoot, TRecursiveRoot>(
            string name,
            Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TBaseRoot, TProfile>> baseQueryBuilder,
            Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TRecursiveRoot, TProfile>> recursiveQueryBuilder)
        {
            _innerQueryBuilder.WithRecursive<TCte, TBaseRoot, TRecursiveRoot>(
                name,
                baseQueryBuilder,
                recursiveQueryBuilder);
            return this;
        }

        /// <inheritdoc />
        public IInsertCommandBuilder<T, TProfile> InsertInto<T>(string tableName)
        {
            return _innerQueryBuilder.InsertInto<T>(tableName);
        }

        /// <inheritdoc />
        public IInsertCommandBuilder<T, TProfile> InsertInto<T>()
        {
            return _innerQueryBuilder.InsertInto<T>();
        }

        /// <inheritdoc />
        public IUpdateCommandBuilder<T> Update<T>(string tableName)
        {
            return _innerQueryBuilder.Update<T>(tableName);
        }

        /// <inheritdoc />
        public IUpdateCommandBuilder<T> Update<T>()
        {
            return _innerQueryBuilder.Update<T>();
        }

        /// <inheritdoc />
        public IDeleteCommandBuilder<T> DeleteFrom<T>(string tableName)
        {
            return _innerQueryBuilder.DeleteFrom<T>(tableName);
        }

        /// <inheritdoc />
        public IDeleteCommandBuilder<T> DeleteFrom<T>()
        {
            return _innerQueryBuilder.DeleteFrom<T>();
        }
    }
}
