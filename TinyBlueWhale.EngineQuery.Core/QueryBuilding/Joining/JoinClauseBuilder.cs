using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Joining
{

    /// <summary>
    /// Builds SQL JOIN definitions.
    /// </summary>
    internal sealed class JoinClauseBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly QuerySourceResolver _sourceResolver = new(context);
        private readonly QuerySourceAliasResolver _aliasResolver = new(context);

        /// <summary>
        /// Adds a metadata-driven JOIN definition.
        /// </summary>
        /// <typeparam name="TSource">
        /// CLR entity type associated with the existing query source.
        /// </typeparam>
        /// <typeparam name="TJoin">
        /// CLR entity type associated with the joined query source.
        /// </typeparam>
        /// <param name="joinType">
        /// SQL join type.
        /// </param>
        /// <param name="alias">
        /// Optional alias associated with the joined source.
        /// </param>
        /// <param name="on">
        /// Expression used to define the JOIN predicate.
        /// </param>
        public void Add<TSource, TJoin>(QueryJoinType joinType, string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            ArgumentNullException.ThrowIfNull(on);

            var sourceDefinition = _aliasResolver.EnsureAlias(
                _sourceResolver.Resolve<TSource>(on.Parameters[0]));

            var joinMetadata = _sourceResolver.ResolveMetadata<TJoin>();

            var joinSource = new QuerySourceDefinition
            {
                EntityType = typeof(TJoin),
                SchemaName = joinMetadata.SchemaName,
                TableName = joinMetadata.TableName,
                TableAlias = alias,
                ColumnMappings = QuerySourceResolver.BuildColumnMappings(joinMetadata)
            };

            _aliasResolver.EnsureAlias(joinSource);

            _context.QueryDefinition.Sources.Add(joinSource);
            _context.QueryDefinition.JoinDefinitions.Add(
                new QueryJoinDefinition
                {
                    JoinType = joinType,
                    Source = sourceDefinition,
                    JoinSource = joinSource,
                    JoinExpression = on
                });
        }

        /// <summary>
        /// Adds an explicit-table JOIN definition.
        /// </summary>
        /// <typeparam name="TSource">
        /// CLR entity type associated with the existing query source.
        /// </typeparam>
        /// <typeparam name="TJoin">
        /// CLR entity type associated with the joined table.
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
        /// Expression used to define the JOIN predicate.
        /// </param>
        public void AddTable<TSource, TJoin>(QueryJoinType joinType, string tableName, string? schemaName, string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
            ArgumentNullException.ThrowIfNull(on);

            if (schemaName is not null)
                ArgumentException.ThrowIfNullOrWhiteSpace(schemaName);

            if (alias is not null)
                ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = _aliasResolver.EnsureAlias(
                _sourceResolver.Resolve<TSource>(on.Parameters[0]));

            var joinSource = new QuerySourceDefinition
            {
                EntityType = typeof(TJoin),
                SchemaName = schemaName,
                TableName = tableName,
                TableAlias = alias,
                ColumnMappings = new Dictionary<string, string>()
            };

            _aliasResolver.EnsureAlias(joinSource);

            _context.QueryDefinition.Sources.Add(joinSource);
            _context.QueryDefinition.JoinDefinitions.Add(
                new QueryJoinDefinition
                {
                    JoinType = joinType,
                    Source = sourceDefinition,
                    JoinSource = joinSource,
                    JoinExpression = on
                });
        }
    }
}
