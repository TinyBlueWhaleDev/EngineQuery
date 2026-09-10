using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.SetOperations;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.SetOperations
{
    /// <summary>
    /// Builds SQL set operation definitions.
    /// </summary>
    /// <typeparam name="TProfile">
    /// Database provider profile associated with the query builder.
    /// </typeparam>
    internal sealed class SetOperationClauseBuilder<TProfile>(
        QueryCommandBuilderContext context,
        TProfile profile)
        where TProfile : IDatabaseProviderProfile
    {
        private readonly QueryCommandBuilderContext _context =
            context ?? throw new ArgumentNullException(nameof(context));

        private readonly TProfile _profile =
            profile ?? throw new ArgumentNullException(nameof(profile));

        /// <summary>
        /// Adds a set operation to the current query definition.
        /// </summary>
        /// <typeparam name="TSet">
        /// Entity type associated with the query on the right side of the set operation.
        /// </typeparam>
        /// <param name="operation">
        /// Set operation applied between the current query and the nested query.
        /// </param>
        /// <param name="setBuilder">
        /// Delegate used to configure the query on the right side of the set operation.
        /// </param>
        public void Add<TSet>(QuerySetOperation operation, Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder)
        {
            ArgumentNullException.ThrowIfNull(setBuilder);

            var nestedQueryBuilder = new QueryBuilder<TProfile>(
                _context.QueryCompiler,
                _context.MetadataResolver,
                _profile);

            var nestedCommandBuilder = setBuilder(nestedQueryBuilder);

            if (nestedCommandBuilder is not QueryCommandBuilder<TSet, TProfile> concreteNestedCommandBuilder)
                throw new InvalidOperationException("The set operation builder returned an unsupported query command builder instance.");

            var nestedQueryDefinition = concreteNestedCommandBuilder.BuildDefinition();

            ValidateProjectionArity(
                operation,
                _context.QueryDefinition,
                nestedQueryDefinition);

            _context.QueryDefinition.SetOperationDefinitions.Add(
                new QuerySetOperationDefinition
                {
                    Operation = operation,
                    Query = nestedQueryDefinition
                });
        }

        // Validates that both sides of a set operation project the same number of columns.
        private static void ValidateProjectionArity(
            QuerySetOperation operation,
            CompiledQueryDefinition queryDefinition,
            CompiledQueryDefinition nestedQueryDefinition)
        {
            var leftArity = GetProjectionArity(queryDefinition);
            var rightArity = GetProjectionArity(nestedQueryDefinition);

            if (leftArity == rightArity)
                return;

            throw new InvalidOperationException(
                $"Set operation '{operation}' requires matching projection arity. " +
                $"The left query projects {leftArity} column(s) and the right query projects {rightArity} column(s).");
        }

        // Gets the number of expressions emitted by the SELECT projection.
        private static int GetProjectionArity(CompiledQueryDefinition queryDefinition)
        {
            if (queryDefinition.UseConstantSelectProjection)
                return 1;

            return queryDefinition.SelectDefinitions.Count
                + queryDefinition.AggregateDefinitions.Count
                + queryDefinition.ScalarFunctionDefinitions.Count
                + queryDefinition.ComputedExpressionDefinitions.Count
                + queryDefinition.CaseWhenDefinitions.Count
                + queryDefinition.WindowFunctionDefinitions.Count;
        }
    }
}
