using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{
    /// <summary>
    /// Validates INSERT command generation across providers.
    /// </summary>
    public static class InsertCommandQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            RunProvider(
                "SQL Server",
                ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver));

            RunProvider(
                "PostgreSQL",
                ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver));

            RunProvider(
                "MySQL",
                ProviderQueryBuilderFactory.CreateMySql(metadataResolver));
        }

        // Runs all INSERT validation scenarios for the specified provider.
        private static void RunProvider(string providerName, QueryBuilder queryBuilder)
        {
            ProviderQueryPrinter.Print(
                $"{providerName} Insert Command",
                BuildInsertValuesQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Insert Select",
                BuildInsertSelectQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Insert Select Where",
                BuildInsertSelectWhereQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Insert Select Join",
                BuildInsertSelectJoinQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Insert Select Inferred Columns",
                BuildInsertSelectInferredColumnsQuery(queryBuilder));
        }

        // Builds a strongly typed INSERT VALUES command.
        private static GeneratedSqlQuery BuildInsertValuesQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .Build();
        }

        // Builds an INSERT SELECT command using explicit target columns.
        private static GeneratedSqlQuery BuildInsertSelectQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Columns(user => new
                {
                    user.Id,
                    user.Email
                })
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Build();
        }

        // Builds an INSERT SELECT command using an explicit WHERE predicate.
        private static GeneratedSqlQuery BuildInsertSelectWhereQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Columns(user => new
                {
                    user.Id,
                    user.Email
                })
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Where<JoinUser>(user => user.IsActive)
                .Build();
        }

        // Builds an INSERT SELECT command using projections from multiple joined sources.
        private static GeneratedSqlQuery BuildInsertSelectJoinQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .InsertInto<JoinOrder>()
                .Columns(order => new
                {
                    order.UserId,
                    order.Total
                })
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (user, order) => user.Id == order.UserId)
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .Select<JoinOrder>(order => new
                {
                    order.Total
                })
                .Where<JoinUser>(user => user.IsActive)
                .Build();
        }

        // Builds an INSERT SELECT command inferring target columns from projection aliases.
        private static GeneratedSqlQuery BuildInsertSelectInferredColumnsQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .InsertInto<JoinOrder>()
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (user, order) => user.Id == order.UserId)
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .Select<JoinOrder>(order => new
                {
                    order.Total
                })
                .Where<JoinUser>(user => user.IsActive)
                .Build();
        }
    }
}

