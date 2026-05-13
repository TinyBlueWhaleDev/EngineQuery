using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Metadata.Fluent;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Dialects;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Dialects;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{ 
    public static class JoinQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = CreateMetadataResolver();

            Print("SQL Server Join", BuildSqlServerQuery(metadataResolver));
            Print("PostgreSQL Join", BuildPostgreSqlQuery(metadataResolver));
            Print("MySQL Join", BuildMySqlQuery(metadataResolver));
        }

        private static GeneratedSqlQuery BuildSqlServerQuery(FluentEntityMetadataResolver metadataResolver)
        {
            var queryBuilder = new QueryBuilder(
                new SqlServerQueryCompiler(new SqlServerDatabaseDialect()),
                metadataResolver);

            return BuildQuery(queryBuilder);
        }

        private static GeneratedSqlQuery BuildPostgreSqlQuery(FluentEntityMetadataResolver metadataResolver)
        {
            var queryBuilder = new QueryBuilder(
                new PostgreSqlQueryCompiler(new PostgreSqlDatabaseDialect()),
                metadataResolver);

            return BuildQuery(queryBuilder);
        }

        private static GeneratedSqlQuery BuildMySqlQuery(FluentEntityMetadataResolver metadataResolver)
        {
            var queryBuilder = new QueryBuilder(
                new MySqlQueryCompiler(new MySqlDatabaseDialect()),
                metadataResolver);

            return BuildQuery(queryBuilder);
        }

        private static GeneratedSqlQuery BuildQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .LeftJoin<JoinOrder, JoinOrderItem>(alias: "oi", on: (o, oi) => o.Id == oi.OrderId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id
                })
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    OrderUserId = o.UserId
                })
                .Select<JoinOrderItem>(oi => new
                {
                    OrderItemId = oi.Id,
                    ItemOrderId = oi.OrderId
                })
                .Build();
        }

        private static FluentEntityMetadataResolver CreateMetadataResolver()
        {
            var registry = new EntityMetadataRegistry();

            registry.Entity<JoinUser>()
                .ToTable("users")
                .Property(x => x.Id).HasColumnName("user_id");

            registry.Entity<JoinOrder>()
                .ToTable("orders")
                .Property(x => x.Id).HasColumnName("order_id")
                .Property(x => x.UserId).HasColumnName("user_id");

            registry.Entity<JoinOrderItem>()
                .ToTable("order_items")
                .Property(x => x.Id).HasColumnName("order_item_id")
                .Property(x => x.OrderId).HasColumnName("order_id");

            return new FluentEntityMetadataResolver(registry);
        }

        private static void Print(string providerName, GeneratedSqlQuery sql)
        {
            Console.WriteLine($"--- {providerName} ---");
            Console.WriteLine(sql.CommandText);
            Console.WriteLine();
        }
    }
}
