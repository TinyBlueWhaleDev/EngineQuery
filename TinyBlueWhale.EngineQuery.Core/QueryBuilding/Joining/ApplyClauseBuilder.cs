using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Subqueries;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;


namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Joining
{
    /// <summary>
    /// Builds APPLY query definitions.
    /// </summary>
    /// <typeparam name="TProfile">
    /// Database provider profile associated with the query builder.
    /// </typeparam>
    internal sealed class ApplyClauseBuilder<TProfile>(QueryCommandBuilderContext context, TProfile profile)
        where TProfile : IDatabaseProviderProfile
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);
        private readonly NestedQueryCommandBuilderFactory<TProfile> _nestedFactory = new(context, profile);
        private readonly QuerySourceAliasResolver _aliasResolver = new(context);

        /// <summary>
        /// Adds an APPLY definition using a nested query correlated with the specified outer source.
        /// </summary>
        /// <typeparam name="TOuter">
        /// Entity type associated with the outer query source.
        /// </typeparam>
        /// <typeparam name="TApply">
        /// Entity type associated with the APPLY subquery source.
        /// </typeparam>
        /// <param name="applyType">
        /// APPLY operation type.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the APPLY result.
        /// </param>
        /// <param name="applyBuilder">
        /// Delegate used to configure the APPLY subquery.
        /// </param>
        public void Add<TOuter, TApply>(QueryApplyType applyType, string alias, Func<IQueryCommandBuilder<TApply, TProfile>, IQueryCommandBuilder<TApply, TProfile>> applyBuilder)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);
            ArgumentNullException.ThrowIfNull(applyBuilder);

            var outerSource = _aliasResolver.EnsureAlias(_sourceResolver.Resolve<TOuter>());

            _context.AliasRegistry.Register(alias);

            var nestedCommandBuilder = _nestedFactory.CreateMetadataBuilder<TApply>(alias);

            nestedCommandBuilder.RegisterOuterSources(
                new[]
                {
                    outerSource
                });

            var configuredBuilder = applyBuilder(nestedCommandBuilder);

            var subqueryDefinition = NestedQueryCommandBuilderFactory<TProfile>.ExtractDefinition(
                configuredBuilder,
                "The APPLY subquery builder returned an unsupported query command builder instance.");

            subqueryDefinition.ForceSelectAliases = true;

            _context.QueryDefinition.ApplyDefinitions.Add(
                new QueryApplyDefinition
                {
                    ApplyType = applyType,
                    Alias = alias,
                    Subquery = subqueryDefinition
                });
        }
    }
}
