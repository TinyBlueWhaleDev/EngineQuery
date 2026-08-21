using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Filtering;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding
{
    /// <summary>
    /// Builds strongly typed SQL DELETE command definitions using a fluent API.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type associated with the target DELETE table.
    /// </typeparam>
    /// <remarks>
    /// This builder does not execute database commands.
    /// It only captures DELETE command intent and delegates SQL generation to the query compiler.
    /// </remarks>
    public sealed class DeleteCommandBuilder<T> : IDeleteCommandBuilder<T>
    {
        private readonly IQueryCompiler _queryCompiler;
        private readonly CompiledQueryDefinition _queryDefinition;
        private readonly WhereClauseBuilder _whereClauseBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandBuilder{T}"/> class.
        /// </summary>
        /// <param name="queryCompiler">
        /// Query compiler used to generate provider-specific command output.
        /// </param>
        /// <param name="tableName">
        /// Database table name associated with the DELETE command.
        /// </param>
        /// <param name="columnMappings">
        /// Optional property-to-column mappings used during SQL generation.
        /// </param>
        /// <param name="metadataResolver">
        /// Optional entity metadata resolver associated with the command.
        /// </param>
        internal DeleteCommandBuilder(IQueryCompiler queryCompiler,string tableName, IReadOnlyDictionary<string, string>? columnMappings = null, IEntityMetadataResolver? metadataResolver = null)
        {
            ArgumentNullException.ThrowIfNull(queryCompiler);
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            _queryCompiler = queryCompiler;

            _queryDefinition = new CompiledQueryDefinition
            {
                CommandType = QueryCommandType.Delete,
                TableName = tableName,
                TableAlias = tableName,
                ColumnMappings = columnMappings ?? new Dictionary<string, string>(),
                EntityType = typeof(T)
            };

            _queryDefinition.SourceDefinitions[typeof(T)] =
                new QuerySourceDefinition
                {
                    EntityType = typeof(T),
                    TableName = tableName,
                    TableAlias = tableName,
                    ColumnMappings = _queryDefinition.ColumnMappings
                };

            var context = new QueryCommandBuilderContext
            {
                QueryCompiler = _queryCompiler,
                QueryDefinition = _queryDefinition,
                MetadataResolver = metadataResolver,
                AliasRegistry = new QueryAliasRegistry()
            };

            context.AliasRegistry.Register(tableName);

            _whereClauseBuilder = new WhereClauseBuilder(context);
        }

        /// <summary>
        /// Adds a WHERE predicate for the target entity.
        /// </summary>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <returns>
        /// Current DELETE command builder instance.
        /// </returns>
        public IDeleteCommandBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            _whereClauseBuilder.Add(predicate);
            return this;
        }

        /// <summary>
        /// Adds a WHERE predicate for the target entity using the specified logical operator.
        /// </summary>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <param name="logicalOperator">
        /// Logical operator used to connect the predicate with the preceding WHERE predicate.
        /// </param>
        /// <returns>
        /// Current DELETE command builder instance.
        /// </returns>
        public IDeleteCommandBuilder<T> Where(Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            _whereClauseBuilder.Add(predicate, logicalOperator);
            return this;
        }

        /// <summary>
        /// Adds an IN collection condition for the target entity.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Property and collection element type.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the property evaluated by the IN condition.
        /// </param>
        /// <param name="values">
        /// Values evaluated by the IN condition.
        /// </param>
        /// <returns>
        /// Current DELETE command builder instance.
        /// </returns>
        public IDeleteCommandBuilder<T> WhereIn<TProperty>(Expression<Func<T, TProperty>> selector, IEnumerable<TProperty> values)
        {
            _whereClauseBuilder.AddCollection(selector, values, isNegated: false);

            return this;
        }

        /// <summary>
        /// Adds a NOT IN collection condition for the target entity.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Property and collection element type.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the property evaluated by the NOT IN condition.
        /// </param>
        /// <param name="values">
        /// Values evaluated by the NOT IN condition.
        /// </param>
        /// <returns>
        /// Current DELETE command builder instance.
        /// </returns>
        public IDeleteCommandBuilder<T> WhereNotIn<TProperty>(Expression<Func<T, TProperty>> selector, IEnumerable<TProperty> values)
        {
            _whereClauseBuilder.AddCollection(selector, values, isNegated: true);

            return this;
        }

        /// <summary>
        /// Adds a filtering expression only when the specified condition is true.
        /// </summary>
        /// <param name="condition">
        /// Determines whether the predicate should be added.
        /// </param>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <returns>
        /// Current DELETE command builder instance.
        /// </returns>
        public IDeleteCommandBuilder<T> WhereIf(bool condition, Expression<Func<T, bool>> predicate)
        {
            _whereClauseBuilder.AddIf(condition, predicate);
            return this;
        }

        /// <summary>
        /// Adds a conditional WHERE predicate using the specified logical operator.
        /// </summary>
        /// <param name="condition">
        /// Determines whether the predicate should be added.
        /// </param>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <param name="logicalOperator">
        /// Logical operator used to connect the predicate with the preceding WHERE predicate.
        /// </param>
        /// <returns>
        /// Current DELETE command builder instance.
        /// </returns>
        public IDeleteCommandBuilder<T> WhereIf(bool condition, Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            _whereClauseBuilder.AddIf(condition, predicate, logicalOperator);

            return this;
        }

        /// <summary>
        /// Compiles the current DELETE definition into SQL command text and parameters.
        /// </summary>
        /// <remarks>
        /// This method only compiles the captured DELETE definition.
        /// It does not execute the generated command against a database.
        /// </remarks>
        /// <returns>
        /// Generated SQL command.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no WHERE predicates were configured.
        /// </exception>
        public GeneratedSqlQuery Build()
        {
            if (_queryDefinition.WhereDefinitions.Count == 0 && _queryDefinition.WhereCollectionDefinitions.Count == 0)
            {
                throw new InvalidOperationException(
                    "At least one WHERE predicate must be configured before building a DELETE command.");
            }

            return _queryCompiler.Compile(_queryDefinition);
        }
    }
}
