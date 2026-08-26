using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Joining
{

    /// <summary>
    /// Builds SQL JOIN definitions.
    /// </summary>
    internal sealed class JoinClauseBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);
        private readonly QuerySourceAliasResolver _aliasResolver = new(context);
        /// <summary>
        /// Adds a metadata-driven JOIN definition.
        /// </summary>
        public void Add<TSource, TJoin>(QueryJoinType joinType, string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            ArgumentNullException.ThrowIfNull(on);

            var sourceDefinition = _aliasResolver.EnsureAlias<TSource>(_sourceResolver.Resolve<TSource>());

            var joinMetadata = _sourceResolver.ResolveMetadata<TJoin>();

            var resolvedAlias = ResolveJoinAlias(alias);

            var joinSource = new QuerySourceDefinition
            {
                EntityType = typeof(TJoin),
                SchemaName = joinMetadata.SchemaName,
                TableName = joinMetadata.TableName,
                TableAlias = resolvedAlias,
                ColumnMappings = QuerySourceResolver.BuildColumnMappings(joinMetadata)
            };

            _context.QueryDefinition.SourceDefinitions[typeof(TJoin)] = joinSource;

            _context.QueryDefinition.JoinDefinitions.Add(
                new QueryJoinDefinition
                {
                    JoinType = joinType,
                    SchemaName = joinSource.SchemaName,
                    TableName = joinSource.TableName!,
                    TableAlias = joinSource.TableAlias,
                    SourceType = typeof(TSource),
                    JoinTypeEntity = typeof(TJoin),
                    JoinExpression = on,
                    SourceAlias = sourceDefinition.TableAlias ?? string.Empty,
                    SourceColumnMappings = sourceDefinition.ColumnMappings,
                    JoinColumnMappings = joinSource.ColumnMappings
                });
        }

        /// <summary>
        /// Adds an explicit-table JOIN definition.
        /// </summary>
        /// <typeparam name="TSource">
        /// Entity type associated with the existing query source.
        /// </typeparam>
        /// <typeparam name="TJoin">
        /// Entity type associated with the joined table.
        /// </typeparam>
        /// <param name="joinType">
        /// SQL join type.
        /// </param>
        /// <param name="tableName">
        /// Physical table name associated with the joined source.
        /// </param>
        /// <param name="schemaName">
        /// Optional database schema associated with the joined table.
        /// </param>
        /// <param name="alias">
        /// Optional alias associated with the joined table.
        /// </param>
        /// <param name="on">
        /// Expression used to define the JOIN condition.
        /// </param>
        public void AddTable<TSource, TJoin>(QueryJoinType joinType, string tableName, string? schemaName, string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
            ArgumentNullException.ThrowIfNull(on);

            if (schemaName is not null)
                ArgumentException.ThrowIfNullOrWhiteSpace(schemaName);

            if (alias is not null)
                ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = _aliasResolver.EnsureAlias<TSource>(_sourceResolver.Resolve<TSource>());
            var resolvedAlias = ResolveJoinAlias(alias);

            var joinSource = new QuerySourceDefinition
            {
                EntityType = typeof(TJoin),
                SchemaName = schemaName,
                TableName = tableName,
                TableAlias = resolvedAlias,
                ColumnMappings = new Dictionary<string, string>()
            };

            _context.QueryDefinition.SourceDefinitions[typeof(TJoin)] = joinSource;

            _context.QueryDefinition.JoinDefinitions.Add(
                new QueryJoinDefinition
                {
                    JoinType = joinType,
                    SchemaName = joinSource.SchemaName,
                    TableName = joinSource.TableName!,
                    TableAlias = joinSource.TableAlias!,
                    SourceType = typeof(TSource),
                    JoinTypeEntity = typeof(TJoin),
                    JoinExpression = on,
                    SourceAlias = sourceDefinition.TableAlias!,
                    SourceColumnMappings = sourceDefinition.ColumnMappings,
                    JoinColumnMappings = joinSource.ColumnMappings
                });
        }

        /// <summary>
        /// Resolves and registers a join alias.
        /// </summary>
        private string ResolveJoinAlias(string? alias)
        {
            var resolvedAlias = string.IsNullOrWhiteSpace(alias)
                ? QueryAliasGeneratorHelper.Generate(_context.AliasRegistry.Count)
                : alias;

            _context.AliasRegistry.Register(resolvedAlias);

            return resolvedAlias;
        }


    }
}
