using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionScopes;
using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Sql.Helpers;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectClauseBuilder"/> class.
    /// </summary>
    /// <param name="columnReferenceBuilder">
    /// SQL column reference builder used to resolve projection columns.
    /// </param>
    public sealed class SelectClauseBuilder(SqlColumnReferenceBuilder columnReferenceBuilder) : IRequiredSqlClauseBuilder
    {
        private readonly SqlColumnReferenceBuilder _columnReferenceBuilder = columnReferenceBuilder ?? throw new ArgumentNullException(nameof(columnReferenceBuilder));

        /// <summary>
        /// Builds the SQL SELECT clause.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition that contains projection metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL SELECT clause.
        /// </returns>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            if (queryDefinition.UseConstantSelectProjection)
                return "SELECT 1";

            if (queryDefinition.SelectDefinitions.Count == 0 &&
                queryDefinition.AggregateDefinitions.Count == 0 &&
                queryDefinition.ScalarFunctionDefinitions.Count == 0 &&
                queryDefinition.ComputedExpressionDefinitions.Count == 0 &&
                queryDefinition.CaseWhenDefinitions.Count == 0 &&
                queryDefinition.WindowFunctionDefinitions.Count == 0)
            {
                return "SELECT *";
            }

            var selectedColumns = queryDefinition.SelectDefinitions
                .Select(selectDefinition => BuildSelectColumn(queryDefinition, selectDefinition, context));

            var aggregateColumns = queryDefinition.AggregateDefinitions
                .Select(BuildAggregateColumn);

            var scalarFunctionColumns = queryDefinition.ScalarFunctionDefinitions
                .Select(functionDefinition => BuildScalarFunctionColumn(functionDefinition, context));

            var computedColumns = queryDefinition.ComputedExpressionDefinitions
                .Select(computedDefinition => BuildComputedExpressionColumn(computedDefinition, context));

            var caseWhenColumns = queryDefinition.CaseWhenDefinitions
                .Select(caseWhenDefinition => BuildCaseWhenColumn(caseWhenDefinition, context));

            var windowFunctionColumns = queryDefinition.WindowFunctionDefinitions
                .Select(windowFunctionDefinition => BuildWindowFunctionColumn(windowFunctionDefinition, context));

            var distinctKeyword = queryDefinition.IsDistinct ? "DISTINCT " : string.Empty;

            return $"SELECT {distinctKeyword}{string.Join(", ", selectedColumns
                .Concat(aggregateColumns)
                .Concat(scalarFunctionColumns)
                .Concat(computedColumns)
                .Concat(caseWhenColumns)
                .Concat(windowFunctionColumns))}";
        }

        private string BuildSelectColumn(CompiledQueryDefinition queryDefinition, QuerySelectColumnDefinition selectDefinition, QueryCompilationContext context)
        {
            var columnReference = BuildSelectColumnReference(queryDefinition, selectDefinition, context);
            var columnName = ResolveSelectColumnName(queryDefinition, selectDefinition);

            var alias = string.IsNullOrWhiteSpace(selectDefinition.Alias)
                ? selectDefinition.PropertyName
                : selectDefinition.Alias;

            var shouldApplyAlias = queryDefinition.ForceSelectAliases ||
                !string.IsNullOrWhiteSpace(selectDefinition.Alias) ||
                !string.Equals(columnName, alias, StringComparison.Ordinal);

            return shouldApplyAlias
                ? $"{columnReference} AS {context.DatabaseDialect.EscapeIdentifier(alias)}"
                : columnReference;
        }

        private string BuildSelectColumnReference(CompiledQueryDefinition queryDefinition, QuerySelectColumnDefinition selectDefinition, QueryCompilationContext context)
        {
            if (selectDefinition.Source is not null)
                return _columnReferenceBuilder.Build(selectDefinition.Source, selectDefinition.PropertyName);

            return QueryColumnMappingHelper.ResolveColumnReference(
                queryDefinition,
                context.DatabaseDialect,
                selectDefinition.PropertyName);
        }

        private static string ResolveSelectColumnName(CompiledQueryDefinition queryDefinition, QuerySelectColumnDefinition selectDefinition)
        {
            if (selectDefinition.Source is not null)
                return SqlColumnReferenceBuilder.ResolveMappedColumnName(selectDefinition.Source.ColumnMappings, selectDefinition.PropertyName);

            return QueryColumnMappingHelper.ResolveColumnName(queryDefinition, selectDefinition.PropertyName);
        }

        private string BuildAggregateColumn(QueryAggregateDefinition aggregateDefinition)
        {
            var columnReference = _columnReferenceBuilder.Build(
                aggregateDefinition.Source,
                aggregateDefinition.PropertyName);

            var functionName = SqlFunctionNameResolver.ResolveAggregateFunctionName(aggregateDefinition.Function);

            return $"{functionName}({columnReference}) AS {_columnReferenceBuilder.Build(aggregateDefinition.Alias, null)}";
        }

        private string BuildScalarFunctionColumn(QueryScalarFunctionDefinition functionDefinition, QueryCompilationContext context)
        {
            var arguments = functionDefinition.Arguments.Count > 0
                ? functionDefinition.Arguments.Select(argument => BuildScalarFunctionArgument(functionDefinition, argument, context))
                : [BuildScalarFunctionSingleColumnArgument(functionDefinition)];

            var functionName = SqlFunctionNameResolver.ResolveScalarFunctionName(
                functionDefinition.Function,
                context.DatabaseDialect);

            return $"{functionName}({string.Join(", ", arguments)}) AS {context.DatabaseDialect.EscapeIdentifier(functionDefinition.Alias)}";
        }

        private string BuildScalarFunctionArgument(
            QueryScalarFunctionDefinition functionDefinition,
            QueryScalarFunctionArgumentDefinition argumentDefinition,
            QueryCompilationContext context)
        {
            if (argumentDefinition.IsColumn)
                return _columnReferenceBuilder.Build(functionDefinition.Source, argumentDefinition.PropertyName!);

            return context.AddParameter(argumentDefinition.ConstantValue);
        }

        private string BuildScalarFunctionSingleColumnArgument(QueryScalarFunctionDefinition functionDefinition)
        {
            if (string.IsNullOrWhiteSpace(functionDefinition.PropertyName))
                throw new InvalidOperationException("Scalar function property name is required for single-column function projections.");

            return _columnReferenceBuilder.Build(functionDefinition.Source, functionDefinition.PropertyName);
        }

        private static string BuildComputedExpressionColumn(QueryComputedExpressionDefinition computedDefinition, QueryCompilationContext context)
        {
            var expressionScope = new QueryExpressionScope();
            expressionScope.Register((ParameterExpression)computedDefinition.Expression.Parameters.Single(), computedDefinition.Source);

            var parser = new SqlComputedExpressionParser(
                context.DatabaseDialect,
                context.Parameters,
                computedDefinition.Source.ColumnMappings,
                computedDefinition.Source.TableAlias,
                expressionScope);

            var sqlExpression = parser.Parse(computedDefinition.Expression.Body);

            return $"{sqlExpression} AS {context.DatabaseDialect.EscapeIdentifier(computedDefinition.Alias)}";
        }

        private static string BuildCaseWhenColumn(QueryCaseWhenDefinition caseWhenDefinition, QueryCompilationContext context)
        {
            var expressionScope = new QueryExpressionScope();
            expressionScope.Register((ParameterExpression)caseWhenDefinition.ConditionExpression.Parameters.Single(), caseWhenDefinition.Source);

            var parser = new SqlComputedExpressionParser(
                context.DatabaseDialect,
                context.Parameters,
                caseWhenDefinition.Source.ColumnMappings,
                caseWhenDefinition.Source.TableAlias,
                expressionScope);

            var conditionSql = parser.Parse(caseWhenDefinition.ConditionExpression.Body);
            var whenTrueParameter = context.AddParameter(caseWhenDefinition.WhenTrueValue);
            var whenFalseParameter = context.AddParameter(caseWhenDefinition.WhenFalseValue);

            return $"CASE WHEN {conditionSql} THEN {whenTrueParameter} ELSE {whenFalseParameter} END AS {context.DatabaseDialect.EscapeIdentifier(caseWhenDefinition.Alias)}";
        }

        private string BuildWindowFunctionColumn(QueryWindowFunctionDefinition windowFunctionDefinition, QueryCompilationContext context)
        {
            var windowClauses = new List<string>(capacity: 2);

            if (windowFunctionDefinition.Partitions.Count > 0)
            {
                var partitionColumns = windowFunctionDefinition.Partitions
                    .Select(partitionDefinition => _columnReferenceBuilder.Build(partitionDefinition.Source, partitionDefinition.Column.PropertyName));

                windowClauses.Add("PARTITION BY " + string.Join(", ", partitionColumns));
            }

            if (windowFunctionDefinition.Orderings.Count > 0)
            {
                var orderingColumns = windowFunctionDefinition.Orderings.Select(orderingDefinition =>
                {
                    var columnReference = _columnReferenceBuilder.Build(
                        orderingDefinition.Source,
                        orderingDefinition.Column.PropertyName);

                    var direction = orderingDefinition.Direction == QueryOrderingDirection.Ascending
                        ? "ASC"
                        : "DESC";

                    return $"{columnReference} {direction}";
                });

                windowClauses.Add("ORDER BY " + string.Join(", ", orderingColumns));
            }

            var functionName = ResolveWindowFunctionName(windowFunctionDefinition.Function);

            var argumentSql = windowFunctionDefinition.Arguments.Count == 0
                ? string.Empty
                : string.Join(", ", windowFunctionDefinition.Arguments
                    .Select(argument => BuildWindowFunctionArgument(argument, context)));

            return $"{functionName}({argumentSql}) OVER ({string.Join(" ", windowClauses)}) AS {context.DatabaseDialect.EscapeIdentifier(windowFunctionDefinition.Alias)}";
        }

        private string BuildWindowFunctionArgument(QueryWindowFunctionArgumentDefinition argumentDefinition, QueryCompilationContext context)
        {
            return argumentDefinition.ArgumentType switch
            {
                QueryWindowFunctionArgumentType.Column => BuildWindowFunctionColumnArgument(argumentDefinition),
                QueryWindowFunctionArgumentType.Constant => context.AddParameter(argumentDefinition.ConstantValue),
                _ => throw new NotSupportedException($"Window function argument type '{argumentDefinition.ArgumentType}' is not supported.")
            };
        }

        private string BuildWindowFunctionColumnArgument(QueryWindowFunctionArgumentDefinition argumentDefinition)
        {
            if (argumentDefinition.Column is null)
                throw new InvalidOperationException("Window function column argument requires a column definition.");

            if (argumentDefinition.Source is null)
                throw new InvalidOperationException("Window function column argument requires a query source.");

            return _columnReferenceBuilder.Build(argumentDefinition.Source, argumentDefinition.Column.PropertyName);
        }

        private static string ResolveWindowFunctionName(QueryWindowFunction function)
        {
            return function switch
            {
                QueryWindowFunction.RowNumber => "ROW_NUMBER",
                QueryWindowFunction.Rank => "RANK",
                QueryWindowFunction.DenseRank => "DENSE_RANK",
                QueryWindowFunction.Lag => "LAG",
                QueryWindowFunction.Lead => "LEAD",
                QueryWindowFunction.FirstValue => "FIRST_VALUE",
                QueryWindowFunction.LastValue => "LAST_VALUE",
                QueryWindowFunction.Ntile => "NTILE",
                _ => throw new NotSupportedException($"Window function '{function}' is not supported.")
            };
        }
    }
}
