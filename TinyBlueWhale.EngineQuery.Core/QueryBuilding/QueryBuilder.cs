using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding
{
    /// <summary>
    /// Default implementation of the query engine responsible for creating query builders.
    /// </summary>
    /// <remarks>
    /// The query engine acts as the main entry point for composing strongly typed SQL queries.
    /// It does not execute queries or manage database connections.
    /// </remarks>
    public sealed class QueryBuilder<TProfile>(IQueryCompiler queryCompiler,
        IEntityMetadataResolver metadataResolver,
        TProfile profile) :
        IQueryBuilder<TProfile>
        where TProfile : IDatabaseProviderProfile
    {
        private readonly IQueryCompiler _queryCompiler = queryCompiler ?? throw new ArgumentNullException(nameof(queryCompiler));

        private readonly IEntityMetadataResolver _metadataResolver = metadataResolver ?? throw new ArgumentNullException(nameof(metadataResolver));

        private readonly TProfile _profile = profile ?? throw new ArgumentNullException(nameof(profile));

        private readonly List<QueryCteDefinition> _cteDefinitions = [];

        /// <summary>
        /// Creates a new query builder using resolved entity metadata.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type used as the source of the query.
        /// </typeparam>       
        /// <returns>
        /// Fluent query command builder.
        /// </returns>
        public IQueryCommandBuilder<T, TProfile> From<T>()
        {
            return CreateCommandBuilder<T>();
        }

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
        public IQueryCommandBuilder<T, TProfile> From<T>(string alias)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            return CreateCommandBuilder<T>(alias);
        }

        /// <summary>
        /// Creates a new query builder using an explicit table name.
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
        /// Fluent query command builder.
        /// </returns>
        public IQueryCommandBuilder<T, TProfile> From<T>(string tableName, string alias)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            return CreateCommandBuilder<T>(tableName, alias);
        }

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
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="alias"/> is null, empty or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="subqueryBuilder"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the derived table subquery builder returns an unsupported query command builder instance.
        /// </exception>
        public IQueryCommandBuilder<TDerived, TProfile> FromSubquery<TDerived, TSubqueryRoot>(string alias, Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSubqueryRoot, TProfile>> subqueryBuilder)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);
            ArgumentNullException.ThrowIfNull(subqueryBuilder);

            var nestedQueryBuilder = new QueryBuilder<TProfile>(_queryCompiler, _metadataResolver, _profile);

            var nestedCommandBuilder = subqueryBuilder(nestedQueryBuilder);

            if (nestedCommandBuilder is not QueryCommandBuilder<TSubqueryRoot, TProfile> concreteNestedCommandBuilder)
                throw new InvalidOperationException("The derived table subquery builder returned an unsupported query command builder instance.");

            var subqueryDefinition = concreteNestedCommandBuilder.BuildDefinition();
            subqueryDefinition.ForceSelectAliases = true;

            var derivedColumnMappings = ResolveDerivedColumnMappings<TDerived>();

            var derivedSource = new QuerySourceDefinition
            {
                EntityType = typeof(TDerived),
                Subquery = subqueryDefinition,
                TableAlias = alias,
                ColumnMappings = derivedColumnMappings
            };

            return new QueryCommandBuilder<TDerived, TProfile>(_queryCompiler, derivedSource, _metadataResolver, _profile);
        }

        /// <summary>
        /// Registers a common table expression that can be used as a query source.
        /// </summary>
        public IQueryBuilder<TProfile> With<TCte, TSubqueryRoot>(string name, Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSubqueryRoot, TProfile>> cteBuilder)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(cteBuilder);

            var nestedQueryBuilder = new QueryBuilder<TProfile>(
                _queryCompiler,
                _metadataResolver,
                _profile);

            var nestedCommandBuilder = cteBuilder(nestedQueryBuilder);

            if (nestedCommandBuilder is not QueryCommandBuilder<TSubqueryRoot, TProfile> concreteNestedCommandBuilder)
                throw new InvalidOperationException("The CTE builder returned an unsupported query command builder instance.");

            var cteQueryDefinition = concreteNestedCommandBuilder.BuildDefinition();
            cteQueryDefinition.ForceSelectAliases = true;

            _cteDefinitions.Add(
                new QueryCteDefinition
                {
                    Name = name,
                    Query = cteQueryDefinition
                });

            return this;
        }

        /// <summary>
        /// Creates a query command builder using a common table expression as the root source.
        /// </summary>
        public IQueryCommandBuilder<TCte, TProfile> FromCte<TCte>(string name, string? alias = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var columnMappings = ResolveDerivedColumnMappings<TCte>();

            var cteSource = new QuerySourceDefinition
            {
                EntityType = typeof(TCte),
                TableName = name,
                TableAlias = alias,
                ColumnMappings = columnMappings
            };

            var commandBuilder = new QueryCommandBuilder<TCte, TProfile>(
                _queryCompiler,
                cteSource,
                _metadataResolver,
                _profile);

            commandBuilder.RegisterCteDefinitions(_cteDefinitions);

            _cteDefinitions.Clear();

            return commandBuilder;
        }

        /// <summary>
        /// Registers a recursive common table expression that can be used as a query source.
        /// </summary>
        /// <typeparam name="TCte">
        /// Entity type associated with the recursive common table expression.
        /// </typeparam>
        /// <typeparam name="TBaseRoot">
        /// Root entity type used by the recursive common table expression base query.
        /// </typeparam>
        /// <typeparam name="TRecursiveRoot">
        /// Root entity type used by the recursive common table expression recursive query.
        /// </typeparam>
        /// <param name="name">
        /// Name assigned to the recursive common table expression.
        /// </param>
        /// <param name="baseQueryBuilder">
        /// Function used to build the recursive common table expression base query.
        /// </param>
        /// <param name="recursiveQueryBuilder">
        /// Function used to build the recursive common table expression recursive query.
        /// </param>
        /// <returns>
        /// Current query builder instance.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="name"/> is null, empty or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="baseQueryBuilder"/> or <paramref name="recursiveQueryBuilder"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the recursive common table expression builders return unsupported query command builder instances.
        /// </exception>
        public IQueryBuilder<TProfile> WithRecursive<TCte, TBaseRoot, TRecursiveRoot>(string name,
            Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TBaseRoot, TProfile>> baseQueryBuilder,
            Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TRecursiveRoot, TProfile>> recursiveQueryBuilder)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(baseQueryBuilder);
            ArgumentNullException.ThrowIfNull(recursiveQueryBuilder);

            var baseBuilder = new QueryBuilder<TProfile>(_queryCompiler, _metadataResolver, _profile);
            var recursiveBuilder = new QueryBuilder<TProfile>(_queryCompiler, _metadataResolver, _profile);

            var baseCommandBuilder = baseQueryBuilder(
                baseBuilder);

            var recursiveCommandBuilder = recursiveQueryBuilder(
                recursiveBuilder);

            if (baseCommandBuilder is not QueryCommandBuilder<TBaseRoot, TProfile> concreteBaseBuilder)
                throw new InvalidOperationException("The recursive CTE base query builder returned an unsupported query command builder instance.");

            if (recursiveCommandBuilder is not QueryCommandBuilder<TRecursiveRoot, TProfile> concreteRecursiveBuilder)
                throw new InvalidOperationException("The recursive CTE recursive query builder returned an unsupported query command builder instance.");

            var baseQueryDefinition = concreteBaseBuilder.BuildDefinition();

            var recursiveQueryDefinition = concreteRecursiveBuilder.BuildDefinition();

            baseQueryDefinition.ForceSelectAliases = true;

            recursiveQueryDefinition.ForceSelectAliases = true;

            baseQueryDefinition.SetOperationDefinitions.Add(
                new QuerySetOperationDefinition
                {
                    Operation = QuerySetOperation.UnionAll,
                    Query = recursiveQueryDefinition
                });

            _cteDefinitions.Add(
                new QueryCteDefinition
                {
                    Name = name,
                    Query = baseQueryDefinition,
                    IsRecursive = true
                });

            return this;
        }

        /// <summary>
        /// Creates a new INSERT command builder using an explicit table name.
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
        public IInsertCommandBuilder<T, TProfile> InsertInto<T>(string tableName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            var metadata = EntityMetadataHelper.Resolve<T>(_metadataResolver);
            var columnMappings = EntityMetadataHelper.CreateColumnMappings(metadata);

            return new InsertCommandBuilder<T, TProfile>(_queryCompiler, _metadataResolver, _profile, tableName, metadata.SchemaName, columnMappings);
        }
        /// <summary>
        /// Creates a new INSERT command builder using resolved entity metadata.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the target INSERT table.
        /// </typeparam>
        /// <returns>
        /// Fluent INSERT command builder.
        /// </returns>
        public IInsertCommandBuilder<T, TProfile> InsertInto<T>()
        {
            var metadata = EntityMetadataHelper.Resolve<T>(_metadataResolver);
            var columnMappings = EntityMetadataHelper.CreateColumnMappings(metadata);

            return new InsertCommandBuilder<T, TProfile>(_queryCompiler, _metadataResolver, _profile, metadata.TableName, metadata.SchemaName, columnMappings);
        }

        /// <summary>
        /// Creates a new UPDATE command builder using an explicit table name.
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
        public IUpdateCommandBuilder<T> Update<T>(string tableName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            var metadata = EntityMetadataHelper.Resolve<T>(_metadataResolver);
            var columnMappings = EntityMetadataHelper.CreateColumnMappings(metadata);

            return new UpdateCommandBuilder<T>(_queryCompiler, _metadataResolver, tableName, metadata.SchemaName, columnMappings);
        }

        /// <summary>
        /// Creates a new UPDATE command builder using resolved entity metadata.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the target UPDATE table.
        /// </typeparam>
        /// <returns>
        /// Fluent UPDATE command builder.
        /// </returns>
        public IUpdateCommandBuilder<T> Update<T>()
        {
            var metadata = EntityMetadataHelper.Resolve<T>(_metadataResolver);
            var columnMappings = EntityMetadataHelper.CreateColumnMappings(metadata);

            return new UpdateCommandBuilder<T>(_queryCompiler, _metadataResolver, metadata.TableName, metadata.SchemaName, columnMappings);
        }

        /// <summary>
        /// Creates a new DELETE command builder using an explicit table name.
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
        public IDeleteCommandBuilder<T> DeleteFrom<T>(string tableName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            var metadata = EntityMetadataHelper.Resolve<T>(_metadataResolver);
            var columnMappings = EntityMetadataHelper.CreateColumnMappings(metadata);

            return new DeleteCommandBuilder<T>(_queryCompiler, _metadataResolver, tableName, metadata.SchemaName, columnMappings);
        }

        /// <summary>
        /// Creates a new DELETE command builder using resolved entity metadata.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the target DELETE table.
        /// </typeparam>
        /// <returns>
        /// Fluent DELETE command builder.
        /// </returns>
        public IDeleteCommandBuilder<T> DeleteFrom<T>()
        {
            var metadata = EntityMetadataHelper.Resolve<T>(_metadataResolver);
            var columnMappings = EntityMetadataHelper.CreateColumnMappings(metadata);

            return new DeleteCommandBuilder<T>(
                _queryCompiler, _metadataResolver, metadata.TableName, metadata.SchemaName, columnMappings);
        }

        /// <summary>
        /// Resolves property-to-column mappings for a derived query source.
        /// </summary>
        /// <typeparam name="TDerived">
        /// CLR type representing the derived query projection.
        /// </typeparam>
        /// <returns>
        /// Metadata-based column mappings when available; otherwise,
        /// property names mapped by convention.
        /// </returns>
        private Dictionary<string, string> ResolveDerivedColumnMappings<TDerived>()
        {
            if (_metadataResolver.TryResolve<TDerived>(out var metadata))
                return EntityMetadataHelper.CreateColumnMappings(metadata!);

            return typeof(TDerived)
                .GetProperties()
                .ToDictionary(property => property.Name, property => property.Name);
        }

        // Creates a query command builder using resolved entity metadata.
        private QueryCommandBuilder<T, TProfile> CreateCommandBuilder<T>()
        {
            var metadata = EntityMetadataHelper.Resolve<T>(_metadataResolver);
            var columnMappings = EntityMetadataHelper.CreateColumnMappings(metadata);

            return new QueryCommandBuilder<T, TProfile>(
                _queryCompiler,
                _metadataResolver,
                _profile,
                metadata.TableName,
                metadata.SchemaName,
                tableAlias: null,
                columnMappings);
        }

        // Creates a query command builder using resolved entity metadata and an alias.
        private QueryCommandBuilder<T, TProfile> CreateCommandBuilder<T>(string alias)
        {
            var metadata = EntityMetadataHelper.Resolve<T>(_metadataResolver);
            var columnMappings = EntityMetadataHelper.CreateColumnMappings(metadata);

            return new QueryCommandBuilder<T, TProfile>(
                _queryCompiler,
                _metadataResolver,
                _profile,
                metadata.TableName,
                metadata.SchemaName,
                alias,
                columnMappings);
        }

        // Creates a query command builder using an explicit table name and alias.
        private QueryCommandBuilder<T, TProfile> CreateCommandBuilder<T>(string tableName, string alias)
        {
            var metadata = EntityMetadataHelper.Resolve<T>(_metadataResolver);
            var columnMappings = EntityMetadataHelper.CreateColumnMappings(metadata);

            return new QueryCommandBuilder<T, TProfile>(
                _queryCompiler,
                _metadataResolver,
                _profile,
                tableName,
                metadata.SchemaName,
                alias,
                columnMappings);
        }

    }
}

