namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Schema
{
    ///// <summary>
    ///// Validates schema-aware SQL generation across supported query commands.
    ///// </summary>
    //[TestFixture]
    //internal sealed class SchemaTests
    //{
    //    private ServiceProvider _serviceProvider = null!;
    //    private IQueryEngine _queryEngine = null!;

    //    /// <summary>
    //    /// Configures EngineQuery with SQL Server and Entity Framework metadata.
    //    /// </summary>
    //    [SetUp]
    //    public void SetUp()
    //    {
    //        var services = new ServiceCollection();

    //        services.AddDbContext<SchemaDbContext>(options =>
    //        {
    //            options.UseInMemoryDatabase(nameof(SchemaDbContext));
    //        });

    //        services.AddEngineQuery(options =>
    //        {
    //            options.Add(QueryEngineProvider.SqlServer, metadata =>
    //            {
    //                metadata.UseEntityFrameworkMetadata<SchemaDbContext>();
    //            });
    //        });

    //        _serviceProvider = services.BuildServiceProvider();
    //        _queryEngine = _serviceProvider.GetRequiredService<IQueryEngine>();
    //    }

    //    /// <summary>
    //    /// Releases services created for the current test.
    //    /// </summary>
    //    [TearDown]
    //    public void TearDown()
    //    {
    //        _serviceProvider.Dispose();
    //    }

    //    /// <summary>
    //    /// Validates schema-qualified JOIN generation using an explicit table source.
    //    /// </summary>
    //    [Test]
    //    public void InnerJoinTable_WhenSchemaIsProvided_ShouldGenerateQualifiedTableName()
    //    {
    //        var query = _queryEngine
    //            .From<SchemaUser>(alias: "u")
    //            .InnerJoinTable<SchemaUser, SchemaProfile>(
    //                tableName: "schema_profiles",
    //                schemaName: "profiles",
    //                alias: "p",
    //                on: (user, profile) => user.Id == profile.UserId)
    //            .Select<SchemaUser>(user => user.Id)
    //            .Select<SchemaProfile>(profile => profile.Id)
    //            .Build();

    //        Assert.That(
    //            query.CommandText,
    //            Does.Contain("INNER JOIN [profiles].[schema_profiles] AS [p]"));
    //    }

    //    /// <summary>
    //    /// Validates schema-qualified JOIN generation using resolved entity metadata.
    //    /// </summary>
    //    [Test]
    //    public void InnerJoin_WhenJoinedEntityHasSchema_ShouldGenerateQualifiedTableName()
    //    {
    //        var query = _queryEngine
    //            .From<SchemaUser>(alias: "u")
    //            .InnerJoin<SchemaUser, SchemaProfile>(
    //                alias: "p",
    //                on: (user, profile) => user.Id == profile.UserId)
    //            .Select<SchemaUser>(user => user.Id)
    //            .Select<SchemaProfile>(profile => profile.Id)
    //            .Build();

    //        Assert.That(
    //            query.CommandText,
    //            Does.Contain("INNER JOIN [profiles].[schema_profiles] AS [p]"));
    //    }

    //    /// <summary>
    //    /// Validates schema-qualified SELECT generation using resolved entity metadata.
    //    /// </summary>
    //    [Test]
    //    public void From_WhenEntityHasSchema_ShouldGenerateQualifiedTableName()
    //    {
    //        var query = _queryEngine
    //            .From<SchemaUser>()
    //            .Select(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("FROM [security].[schema_users]"));

    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("[schema_user_id]"));

    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("[email]"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates schema-qualified INSERT generation using resolved entity metadata.
    //    /// </summary>
    //    [Test]
    //    public void InsertInto_WhenEntityHasSchema_ShouldGenerateQualifiedTableName()
    //    {
    //        var query = _queryEngine
    //            .InsertInto<SchemaUser>()
    //            .Set(user => user.Email, "test@test.com")
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("INSERT INTO [security].[schema_users]"));

    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("[email]"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates schema-qualified UPDATE generation using resolved entity metadata.
    //    /// </summary>
    //    [Test]
    //    public void Update_WhenEntityHasSchema_ShouldGenerateQualifiedTableName()
    //    {
    //        var query = _queryEngine
    //            .Update<SchemaUser>()
    //            .Set(user => user.Email, "updated@test.com")
    //            .Where(user => user.Id == 10)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("UPDATE [security].[schema_users]"));

    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("[email]"));

    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("[schema_user_id]"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates schema-qualified DELETE generation using resolved entity metadata.
    //    /// </summary>
    //    [Test]
    //    public void DeleteFrom_WhenEntityHasSchema_ShouldGenerateQualifiedTableName()
    //    {
    //        var query = _queryEngine
    //            .DeleteFrom<SchemaUser>()
    //            .Where(user => user.Id == 10)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("FROM [security].[schema_users]"));

    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("[schema_user_id]"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates independent target and source schemas for INSERT SELECT.
    //    /// </summary>
    //    [Test]
    //    public void InsertSelect_WhenTargetAndSourceHaveDifferentSchemas_ShouldGenerateQualifiedTableNames()
    //    {
    //        var query = _queryEngine
    //            .InsertInto<ArchiveUser>()
    //            .Columns(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .From<SchemaUser>()
    //            .Select<SchemaUser>(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("INSERT INTO [archive].[archived_users]"));

    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("FROM [security].[schema_users]"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that an explicit SELECT table name overrides the metadata table
    //    /// while preserving the resolved schema and column mappings.
    //    /// </summary>
    //    [Test]
    //    public void From_WhenExplicitTableNameIsProvided_ShouldPreserveMetadataSchema()
    //    {
    //        var query = _queryEngine
    //            .From<SchemaUser>("schema_users_archive", "su")
    //            .Select(user => user.Id)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("FROM [security].[schema_users_archive]"));

    //            Assert.That(
    //                query.CommandText,
    //                Does.Not.Contain("AS [schema_users_archive]"));

    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("[schema_user_id]"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that an explicit SELECT table name can be combined with
    //    /// an explicit alias while preserving the resolved schema.
    //    /// </summary>
    //    [Test]
    //    public void From_WhenExplicitTableNameAndAliasAreProvided_ShouldPreserveSchemaAndAlias()
    //    {
    //        var query = _queryEngine
    //            .From<SchemaUser>(
    //                "schema_users_archive",
    //                alias: "u")
    //            .Select(user => user.Id)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("FROM [security].[schema_users_archive] AS [u]"));

    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("[u].[schema_user_id]"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that an explicit INSERT table name overrides the metadata table
    //    /// while preserving the resolved schema and column mappings.
    //    /// </summary>
    //    [Test]
    //    public void InsertInto_WhenExplicitTableNameIsProvided_ShouldPreserveMetadataSchema()
    //    {
    //        var query = _queryEngine
    //            .InsertInto<SchemaUser>("schema_users_archive")
    //            .Set(user => user.Email, "test@test.com")
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("INSERT INTO [security].[schema_users_archive]"));

    //            Assert.That(
    //                query.CommandText,
    //                Does.Contain("[email]"));
    //        });
    //    }

    //    /// <summary>
    //    /// Entity Framework context used to provide schema-aware metadata.
    //    /// </summary>
    //    private sealed class SchemaDbContext(
    //        DbContextOptions<SchemaDbContext> options)
    //        : DbContext(options)
    //    {
    //        /// <summary>
    //        /// Configures physical tables, schemas and column mappings used by schema tests.
    //        /// </summary>
    //        /// <param name="modelBuilder">
    //        /// Entity Framework model builder.
    //        /// </param>
    //        protected override void OnModelCreating(ModelBuilder modelBuilder)
    //        {
    //            modelBuilder.Entity<SchemaProfile>(entity =>
    //            {
    //                entity.ToTable(
    //                    "schema_profiles",
    //                    "profiles");

    //                entity.HasKey(profile => profile.Id);

    //                entity.Property(profile => profile.Id)
    //                    .HasColumnName("profile_id");

    //                entity.Property(profile => profile.UserId)
    //                    .HasColumnName("user_id");
    //            });

    //            modelBuilder.Entity<SchemaUser>(entity =>
    //            {
    //                entity.ToTable(
    //                    "schema_users",
    //                    "security");

    //                entity.HasKey(user => user.Id);

    //                entity.Property(user => user.Id)
    //                    .HasColumnName("schema_user_id");

    //                entity.Property(user => user.Email)
    //                    .HasColumnName("email");
    //            });

    //            modelBuilder.Entity<ArchiveUser>(entity =>
    //            {
    //                entity.ToTable(
    //                    "archived_users",
    //                    "archive");

    //                entity.HasKey(user => user.Id);

    //                entity.Property(user => user.Id)
    //                    .HasColumnName("archive_user_id");

    //                entity.Property(user => user.Email)
    //                    .HasColumnName("email");
    //            });
    //        }
    //    }

    //    /// <summary>
    //    /// Entity mapped to the profiles schema for JOIN validation.
    //    /// </summary>
    //    private sealed class SchemaProfile
    //    {
    //        /// <summary>
    //        /// Gets or initializes the profile identifier.
    //        /// </summary>
    //        public int Id { get; init; }

    //        /// <summary>
    //        /// Gets or initializes the related user identifier.
    //        /// </summary>
    //        public int UserId { get; init; }
    //    }

    //    /// <summary>
    //    /// Source entity mapped to the security schema.
    //    /// </summary>
    //    private sealed class SchemaUser
    //    {
    //        /// <summary>
    //        /// Gets or initializes the user identifier.
    //        /// </summary>
    //        public int Id { get; init; }

    //        /// <summary>
    //        /// Gets or initializes the user email address.
    //        /// </summary>
    //        public string? Email { get; init; }
    //    }

    //    /// <summary>
    //    /// Target entity mapped to the archive schema.
    //    /// </summary>
    //    private sealed class ArchiveUser
    //    {
    //        /// <summary>
    //        /// Gets or initializes the archived user identifier.
    //        /// </summary>
    //        public int Id { get; init; }

    //        /// <summary>
    //        /// Gets or initializes the archived user email address.
    //        /// </summary>
    //        public string? Email { get; init; }
    //    }
    //}
}
