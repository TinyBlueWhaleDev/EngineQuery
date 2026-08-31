using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TinyBlueWhale.EngineQuery.DependencyInjection.Extensions;
using TinyBlueWhale.EngineQuery.Metadata.EntityFramework;

namespace TinyBlueWhale.EngineQuery.Tests.DependencyInjection
{
    ///// <summary>
    ///// Validates convention metadata fallback behavior during dependency injection resolution.
    ///// </summary>
    //[TestFixture]
    //internal sealed class MetadataFallbackTests
    //{
    //    /// <summary>
    //    /// Validates that an explicit SELECT table name preserves
    //    /// convention-based column mappings.
    //    /// </summary>
    //    [Test]
    //    public void From_WhenExplicitTableNameIsUsed_ShouldPreserveConventionColumnMappings()
    //    {
    //        using var serviceProvider = CreateConventionServiceProvider();

    //        var queryEngine = serviceProvider.GetRequiredService<IQueryEngine>();

    //        var query = queryEngine
    //            .From<ConventionUser>("custom_users")
    //            .Select(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("[custom_users]"));
    //            Assert.That(query.CommandText, Does.Contain("[Id]"));
    //            Assert.That(query.CommandText, Does.Contain("[Email]"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that an explicit INSERT table name preserves
    //    /// convention-based column mappings.
    //    /// </summary>
    //    [Test]
    //    public void InsertInto_WhenExplicitTableNameIsUsed_ShouldPreserveConventionColumnMappings()
    //    {
    //        using var serviceProvider = CreateConventionServiceProvider();

    //        var queryEngine = serviceProvider.GetRequiredService<IQueryEngine>();

    //        var query = queryEngine
    //            .InsertInto<ConventionUser>("custom_users")
    //            .Set(user => user.Email, "test@test.com")
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("[custom_users]"));
    //            Assert.That(query.CommandText, Does.Contain("[Email]"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that an explicit UPDATE table name preserves
    //    /// convention-based column mappings.
    //    /// </summary>
    //    [Test]
    //    public void Update_WhenExplicitTableNameIsUsed_ShouldPreserveConventionColumnMappings()
    //    {
    //        using var serviceProvider = CreateConventionServiceProvider();

    //        var queryEngine = serviceProvider.GetRequiredService<IQueryEngine>();

    //        var query = queryEngine
    //            .Update<ConventionUser>("custom_users")
    //            .Set(user => user.Email, "updated@test.com")
    //            .Where(user => user.Id == 10)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("[custom_users]"));
    //            Assert.That(query.CommandText, Does.Contain("[Email]"));
    //            Assert.That(query.CommandText, Does.Contain("[Id]"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that an explicit DELETE table name preserves
    //    /// convention-based column mappings.
    //    /// </summary>
    //    [Test]
    //    public void DeleteFrom_WhenExplicitTableNameIsUsed_ShouldPreserveConventionColumnMappings()
    //    {
    //        using var serviceProvider = CreateConventionServiceProvider();

    //        var queryEngine = serviceProvider.GetRequiredService<IQueryEngine>();

    //        var query = queryEngine
    //            .DeleteFrom<ConventionUser>("custom_users")
    //            .Where(user => user.Id == 10)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("[custom_users]"));
    //            Assert.That(query.CommandText, Does.Contain("[Id]"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that convention metadata is used when no explicit
    //    /// metadata strategy is configured.
    //    /// </summary>
    //    [Test]
    //    public void AddEngineQuery_WhenMetadataIsNotConfigured_ShouldUseConventionMetadata()
    //    {
    //        using var serviceProvider = CreateConventionServiceProvider();

    //        var queryEngine = serviceProvider.GetRequiredService<IQueryEngine>();

    //        var query = queryEngine
    //            .From<ConventionUser>()
    //            .Select(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("[ConventionUser]"));
    //            Assert.That(query.CommandText, Does.Contain("[Id]"));
    //            Assert.That(query.CommandText, Does.Contain("[Email]"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates convention fallback when Entity Framework metadata
    //    /// cannot resolve the requested entity.
    //    /// </summary>
    //    [Test]
    //    public void AddEngineQuery_WhenEntityFrameworkCannotResolveEntity_ShouldUseConventionFallback()
    //    {
    //        var services = new ServiceCollection();

    //        services.AddDbContext<MetadataFallbackDbContext>(options =>
    //        {
    //            options.UseInMemoryDatabase(nameof(MetadataFallbackDbContext));
    //        });

    //        services.AddEngineQuery(options =>
    //        {
    //            options.Add(QueryEngineProvider.SqlServer, metadata =>
    //            {
    //                metadata.UseEntityFrameworkMetadata<MetadataFallbackDbContext>();
    //            });
    //        });

    //        using var serviceProvider = services.BuildServiceProvider();

    //        var queryEngine = serviceProvider.GetRequiredService<IQueryEngine>();

    //        var query = queryEngine
    //            .From<ConventionUser>()
    //            .Select(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("[ConventionUser]"));
    //            Assert.That(query.CommandText, Does.Contain("[Id]"));
    //            Assert.That(query.CommandText, Does.Contain("[Email]"));
    //        });
    //    }

    //    /// <summary>
    //    /// Creates a service provider configured with convention metadata.
    //    /// </summary>
    //    /// <returns>
    //    /// Service provider containing a convention-based EngineQuery registration.
    //    /// </returns>
    //    private static ServiceProvider CreateConventionServiceProvider()
    //    {
    //        var services = new ServiceCollection();

    //        services.AddEngineQuery(options =>
    //        {
    //            options.Add(QueryEngineProvider.SqlServer);
    //        });

    //        return services.BuildServiceProvider();
    //    }

    //    /// <summary>
    //    /// Entity used to validate convention metadata fallback behavior.
    //    /// </summary>
    //    private sealed class ConventionUser
    //    {
    //        /// <summary>
    //        /// Gets or initializes the user identifier.
    //        /// </summary>
    //        public int Id { get; init; }

    //        /// <summary>
    //        /// Gets or initializes the user email.
    //        /// </summary>
    //        public string? Email { get; init; }
    //    }

    //    /// <summary>
    //    /// Entity Framework context intentionally excluding
    //    /// <see cref="ConventionUser"/> from its model.
    //    /// </summary>
    //    private sealed class MetadataFallbackDbContext(
    //        DbContextOptions<MetadataFallbackDbContext> options)
    //        : DbContext(options)
    //    {
    //    }
    //}
}
