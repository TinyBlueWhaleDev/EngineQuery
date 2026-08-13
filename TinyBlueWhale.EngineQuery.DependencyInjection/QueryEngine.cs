using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces;

namespace TinyBlueWhale.EngineQuery.DependencyInjection
{
    /// <summary>
    /// Default configured EngineQuery implementation.
    /// </summary>
    internal sealed class QueryEngine(IQueryBuilder innerQueryBuilder) : IQueryEngine
    {
        private readonly IQueryBuilder _innerQueryBuilder = innerQueryBuilder ?? throw new ArgumentNullException(nameof(innerQueryBuilder));

        /// <inheritdoc />
        public IQueryCommandBuilder<T> From<T>(string tableName, string? alias = null)
        {
            return _innerQueryBuilder.From<T>(tableName, alias);
        }

        /// <inheritdoc />
        public IQueryCommandBuilder<T> From<T>(string? alias = null)
        {
            return _innerQueryBuilder.From<T>(alias);
        }

        /// <inheritdoc />
        public IQueryCommandBuilder<TDerived> FromSubquery<TDerived, TSubqueryRoot>(
            string alias,
            Func<IQueryBuilder, IQueryCommandBuilder<TSubqueryRoot>> subqueryBuilder)
        {
            return _innerQueryBuilder.FromSubquery<TDerived, TSubqueryRoot>(alias, subqueryBuilder);
        }

        /// <inheritdoc />
        public IQueryBuilder With<TCte, TSubqueryRoot>(
            string name,
            Func<IQueryBuilder, IQueryCommandBuilder<TSubqueryRoot>> cteBuilder)
        {
            _innerQueryBuilder.With<TCte, TSubqueryRoot>(name, cteBuilder);
            return this;
        }

        /// <inheritdoc />
        public IQueryCommandBuilder<TCte> FromCte<TCte>(string name)
        {
            return _innerQueryBuilder.FromCte<TCte>(name);
        }

        /// <inheritdoc />
        public IQueryBuilder WithRecursive<TCte, TBaseRoot, TRecursiveRoot>(
            string name,
            Func<IQueryBuilder, IQueryCommandBuilder<TBaseRoot>> baseQueryBuilder,
            Func<IQueryBuilder, IQueryCommandBuilder<TRecursiveRoot>> recursiveQueryBuilder)
        {
            _innerQueryBuilder.WithRecursive<TCte, TBaseRoot, TRecursiveRoot>(
                name,
                baseQueryBuilder,
                recursiveQueryBuilder);
            return this;
        }

        /// <inheritdoc />
        public IInsertCommandBuilder<T> InsertInto<T>(string tableName)
        {
            return _innerQueryBuilder.InsertInto<T>(tableName);
        }

        /// <inheritdoc />
        public IInsertCommandBuilder<T> InsertInto<T>()
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
