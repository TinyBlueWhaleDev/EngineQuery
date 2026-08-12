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
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding
{
    /// <summary>
    /// Builds strongly typed SQL INSERT command definitions using a fluent API.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type associated with the target INSERT table.
    /// </typeparam>
    /// <remarks>
    /// This builder does not execute database commands.
    /// It only captures INSERT command intent and delegates SQL generation to the query compiler.
    /// </remarks>
    public sealed class InsertCommandBuilder<T> : IInsertCommandBuilder<T>
    {
        private readonly IQueryCompiler _queryCompiler;
        private readonly CompiledQueryDefinition _queryDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandBuilder{T}"/> class.
        /// </summary>
        /// <param name="queryCompiler">
        /// Query compiler used to generate provider-specific command output.
        /// </param>
        /// <param name="tableName">
        /// Database table name associated with the INSERT command.
        /// </param>
        /// <param name="columnMappings">
        /// Optional property-to-column mappings used during SQL generation.
        /// </param>
        internal InsertCommandBuilder(IQueryCompiler queryCompiler, string tableName,
            IReadOnlyDictionary<string, string>? columnMappings = null)
        {
            ArgumentNullException.ThrowIfNull(queryCompiler);
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            _queryCompiler = queryCompiler;

            _queryDefinition = new CompiledQueryDefinition
            {
                CommandType = QueryCommandType.Insert,
                TableName = tableName,
                ColumnMappings = columnMappings ?? new Dictionary<string, string>(),
                EntityType = typeof(T),
                InsertDefinition = new QueryInsertDefinition()
            };
        }

        /// <summary>
        /// Adds a value assignment for the selected entity property.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Property type associated with the inserted value.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the target entity property.
        /// </param>
        /// <param name="value">
        /// Value assigned to the selected property.
        /// </param>
        /// <returns>
        /// Current INSERT command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="selector"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the selector does not reference a direct entity property.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the selected property was already assigned.
        /// </exception>
        public IInsertCommandBuilder<T> Set<TProperty>(Expression<Func<T, TProperty>> selector, TProperty value)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var propertyName = ResolvePropertyName(selector);

            if (_queryDefinition.InsertDefinition!.ValueDefinitions.Any(definition => definition.ColumnName.Equals(propertyName, StringComparison.Ordinal)))
                throw new InvalidOperationException($"Property '{propertyName}' already has an INSERT value assignment.");


            var columnName = _queryDefinition.ColumnMappings.TryGetValue(propertyName, out var mappedColumnName)
                ? mappedColumnName
                : propertyName;

            _queryDefinition.InsertDefinition.ValueDefinitions.Add(
                new QueryInsertValueDefinition
                {
                    ColumnName = columnName,
                    Value = value
                });

            return this;
        }

        /// <summary>
        /// Compiles the current INSERT definition into SQL command text and parameters.
        /// </summary>
        /// <remarks>
        /// This method only compiles the captured INSERT definition.
        /// It does not execute the generated command against a database.
        /// </remarks>
        /// <returns>
        /// Generated SQL command.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no INSERT value assignments were configured.
        /// </exception>
        public GeneratedSqlQuery Build()
        {
            if (_queryDefinition.InsertDefinition!.ValueDefinitions.Count == 0)
                throw new InvalidOperationException("At least one value must be configured before building an INSERT command.");

            return _queryCompiler.Compile(_queryDefinition);
        }

        // Resolves the selected entity property name from an INSERT value assignment expression.
        private static string ResolvePropertyName<TProperty>(Expression<Func<T, TProperty>> selector)
        {
            Expression expression = selector.Body;

            if (expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
                expression = unaryExpression.Operand;

            if (expression is not MemberExpression memberExpression || memberExpression.Expression is not ParameterExpression)
                throw new ArgumentException("The INSERT selector must reference a direct entity property.", nameof(selector));


            return memberExpression.Member.Name;
        }
    }
}
