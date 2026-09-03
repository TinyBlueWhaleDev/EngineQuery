using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Metadata.Fluent;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{
    public static class MultiProviderQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = CreateMetadataResolver();

            Print("SQL Server",
                BuildSqlServerQuery(metadataResolver));

            Print("PostgreSQL",
                BuildPostgreSqlQuery(metadataResolver));

            Print("MySQL",
                BuildMySqlQuery(metadataResolver));
        }

        private static GeneratedSqlQuery BuildSqlServerQuery(FluentEntityMetadataResolver metadataResolver)
        {
            var queryBuilder = SqlServerQueryCompiler.Factory.Create(metadataResolver);

            return BuildQuery(queryBuilder);
        }

        private static GeneratedSqlQuery BuildPostgreSqlQuery(FluentEntityMetadataResolver metadataResolver)
        {
            var queryBuilder = PostgreSqlQueryCompiler.Factory.Create(metadataResolver);

            return BuildQuery(queryBuilder);
        }

        private static GeneratedSqlQuery BuildMySqlQuery(FluentEntityMetadataResolver metadataResolver)
        {
            var queryBuilder = MySqlQueryCompiler.Factory.Create(metadataResolver);

            return BuildQuery(queryBuilder);
        }

        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<FluentAuditRecord>()
                .Select(x => new
                {
                    LogId = x.AuditId,
                    Message = x.Description,
                    CreatedAt = x.CreatedOn,
                    IsActive = x.Active
                })
                .Where(x =>
                    x.Active &&
                    x.Description.Contains("error"))
                .OrderByDescending(x => x.CreatedOn)
                .Build();
        }

        private static FluentEntityMetadataResolver CreateMetadataResolver()
        {
            var registry = new EntityMetadataRegistry();

            registry.Entity<FluentAuditRecord>()
                .ToTable("system_logs")
                .Property(x => x.AuditId).HasColumnName("log_id")
                .Property(x => x.Description).HasColumnName("message_text")
                .Property(x => x.CreatedOn).HasColumnName("created_at")
                .Property(x => x.Active).HasColumnName("is_active");

            return new FluentEntityMetadataResolver(registry);
        }

        private static void Print(string providerName, GeneratedSqlQuery sql)
        {
            Console.WriteLine($"--- {providerName} ---");
            Console.WriteLine(sql.CommandText);

            foreach (var parameter in sql.Parameters)
            {
                Console.WriteLine($"{parameter.Name} = {parameter.Value}");
            }

            Console.WriteLine();
        }
    }
}
