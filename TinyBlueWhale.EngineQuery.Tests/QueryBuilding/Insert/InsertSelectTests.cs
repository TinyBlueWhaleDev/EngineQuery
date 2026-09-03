namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Insert
{
    /// <summary>
    /// Validates provider-independent INSERT SELECT behavior.
    /// </summary>
    //[TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    //internal sealed class InsertSelectTests(IQueryTestProvider provider)
    //{
    //    private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));


    //    /// <summary>
    //    /// Validates INSERT SELECT generation using explicitly configured target columns.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenInsertSelectIsValid_ShouldGenerateExpectedSql()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinUser>()
    //            .Columns(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .From<JoinUser>(alias: "u")
    //            .Select<JoinUser>(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .Where<JoinUser>(user => user.IsActive)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("INSERT"));
    //            Assert.That(query.CommandText, Does.Contain("users"));
    //            Assert.That(query.CommandText, Does.Contain("SELECT"));
    //            Assert.That(query.CommandText, Does.Contain("user_id"));
    //            Assert.That(query.CommandText, Does.Contain("email"));
    //            Assert.That(query.CommandText, Does.Contain("WHERE"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates INSERT SELECT generation using a joined source.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenJoinIsConfigured_ShouldGenerateJoin()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinOrder>()
    //            .Columns(order => new
    //            {
    //                order.UserId,
    //                order.Total
    //            })
    //            .From<JoinUser>(alias: "u")
    //            .InnerJoin<JoinUser, JoinOrder>(
    //                alias: "o",
    //                on: (user, order) => user.Id == order.UserId)
    //            .Select<JoinUser>(user => new
    //            {
    //                UserId = user.Id
    //            })
    //            .Select<JoinOrder>(order => new
    //            {
    //                order.Total
    //            })
    //            .Where<JoinUser>(user => user.IsActive)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("INSERT"));
    //            Assert.That(query.CommandText, Does.Contain("orders"));
    //            Assert.That(query.CommandText, Does.Contain("INNER JOIN"));
    //            Assert.That(query.CommandText, Does.Contain("users"));
    //            Assert.That(query.CommandText, Does.Contain("total"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that INSERT SELECT preserves the configured INSERT target
    //    /// when the SELECT root source uses a different entity type.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenSourceUsesDifferentEntity_ShouldPreserveInsertTarget()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinOrder>()
    //            .Columns(order => new
    //            {
    //                order.UserId,
    //                order.Total
    //            })
    //            .From<JoinUser>(alias: "u")
    //            .InnerJoin<JoinUser, JoinOrder>(
    //                alias: "o",
    //                on: (user, order) => user.Id == order.UserId)
    //            .Select<JoinUser>(user => new
    //            {
    //                UserId = user.Id
    //            })
    //            .Select<JoinOrder>(order => new
    //            {
    //                order.Total
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("orders"));
    //            Assert.That(query.CommandText, Does.Contain("users"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates INSERT SELECT target column inference from direct projections.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenColumnsAreNotConfigured_ShouldInferProjectionColumns()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinOrder>()
    //            .From<JoinUser>(alias: "u")
    //            .InnerJoin<JoinUser, JoinOrder>(
    //                alias: "o",
    //                on: (user, order) => user.Id == order.UserId)
    //            .Select<JoinUser>(user => new
    //            {
    //                UserId = user.Id
    //            })
    //            .Select<JoinOrder>(order => new
    //            {
    //                order.Total
    //            })
    //            .Where<JoinUser>(user => user.IsActive)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("UserId"));
    //            Assert.That(query.CommandText, Does.Contain("Total"));
    //            Assert.That(query.CommandText, Does.Contain("SELECT"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates INSERT SELECT target column inference from property names.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenProjectionHasNoAliases_ShouldInferPropertyNames()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinUser>()
    //            .From<JoinUser>(alias: "u")
    //            .Select<JoinUser>(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("Id"));
    //            Assert.That(query.CommandText, Does.Contain("Email"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates INSERT SELECT target column inference from aggregate projection aliases.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenAggregateProjectionIsUsed_ShouldInferAlias()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinOrder>("projection_results")
    //            .From<JoinOrder>(alias: "o")
    //            .SelectAggregate<JoinOrder>(
    //                QueryAggregateFunction.Sum,
    //                order => order.Total,
    //                "TotalAmount")
    //            .Build();

    //        Assert.That(query.CommandText, Does.Contain("TotalAmount"));
    //    }

    //    /// <summary>
    //    /// Validates INSERT SELECT target column inference from scalar function aliases.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenScalarFunctionProjectionIsUsed_ShouldInferAlias()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinUser>("projection_results")
    //            .From<JoinUser>(alias: "u")
    //            .SelectScalarFunction<JoinUser>(
    //                QueryScalarFunction.Upper,
    //                user => user.Email,
    //                "NormalizedEmail")
    //            .Build();

    //        Assert.That(query.CommandText, Does.Contain("NormalizedEmail"));
    //    }

    //    /// <summary>
    //    /// Validates INSERT SELECT target column inference from computed expression aliases.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenComputedProjectionIsUsed_ShouldInferAlias()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinOrder>("projection_results")
    //            .From<JoinOrder>(alias: "o")
    //            .SelectComputed<JoinOrder>(
    //                order => order.Total * 1.16m,
    //                "TotalWithTax")
    //            .Build();

    //        Assert.That(query.CommandText, Does.Contain("TotalWithTax"));
    //    }

    //    /// <summary>
    //    /// Validates INSERT SELECT target column inference from CASE WHEN aliases.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenCaseProjectionIsUsed_ShouldInferAlias()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinOrder>("projection_results")
    //            .From<JoinOrder>(alias: "o")
    //            .SelectCaseWhen<JoinOrder>(
    //                order => order.Total > 1000,
    //                "VIP",
    //                "STANDARD",
    //                "CustomerType")
    //            .Build();

    //        Assert.That(query.CommandText, Does.Contain("CustomerType"));
    //    }

    //    /// <summary>
    //    /// Validates that INSERT SELECT column inference requires at least one projection.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenInferredColumnsHaveNoProjection_ShouldThrow()
    //    {
    //        var commandBuilder = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinUser>()
    //            .From<JoinUser>(alias: "u");

    //        var exception = Assert.Throws<InvalidOperationException>(() => commandBuilder.Build());

    //        Assert.That(
    //            exception!.Message,
    //            Is.EqualTo("At least one SELECT projection must be configured when INSERT target columns are not explicitly configured."));
    //    }

    //    /// <summary>
    //    /// Validates that inferred INSERT target columns cannot contain duplicate projection names.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenInferredTargetColumnIsDuplicated_ShouldThrow()
    //    {
    //        var commandBuilder = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinOrder>()
    //            .From<JoinUser>(alias: "u")
    //            .InnerJoin<JoinUser, JoinOrder>(
    //                alias: "o",
    //                on: (user, order) => user.Id == order.UserId)
    //            .Select<JoinUser>(user => new
    //            {
    //                UserId = user.Id
    //            })
    //            .Select<JoinOrder>(order => new
    //            {
    //                UserId = order.UserId
    //            });

    //        var exception = Assert.Throws<InvalidOperationException>(() => commandBuilder.Build());

    //        Assert.That(
    //            exception!.Message,
    //            Is.EqualTo("Target INSERT column 'UserId' was resolved more than once from the SELECT projection."));
    //    }

    //    /// <summary>
    //    /// Validates that duplicate inferred target columns across projection types are rejected.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenProjectionTypesResolveSameTargetColumn_ShouldThrow()
    //    {
    //        var commandBuilder = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinOrder>("projection_results")
    //            .From<JoinOrder>(alias: "o")
    //            .Select<JoinOrder>(order => new
    //            {
    //                TotalAmount = order.Total
    //            })
    //            .SelectAggregate<JoinOrder>(
    //                QueryAggregateFunction.Sum,
    //                order => order.Total,
    //                "TotalAmount");

    //        var exception = Assert.Throws<InvalidOperationException>(() => commandBuilder.Build());

    //        Assert.That(
    //            exception!.Message,
    //            Is.EqualTo("Target INSERT column 'TotalAmount' was resolved more than once from the SELECT projection."));
    //    }

    //    /// <summary>
    //    /// Validates that INSERT value assignments cannot be combined
    //    /// with explicitly configured INSERT SELECT columns.
    //    /// </summary>
    //    [Test]
    //    public void Set_WhenColumnsAreAlreadyConfigured_ShouldThrow()
    //    {
    //        var commandBuilder = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinUser>()
    //            .Columns(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            });

    //        var exception = Assert.Throws<InvalidOperationException>(() =>
    //            commandBuilder.Set(user => user.Email, "admin@test.com"));

    //        Assert.That(
    //            exception!.Message,
    //            Is.EqualTo("INSERT value assignments cannot be combined with explicitly configured INSERT SELECT columns."));
    //    }

    //    /// <summary>
    //    /// Validates that an INSERT SELECT target column cannot be configured more than once.
    //    /// </summary>
    //    [Test]
    //    public void Columns_WhenTargetColumnIsConfiguredMoreThanOnce_ShouldThrow()
    //    {
    //        var commandBuilder = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinUser>()
    //            .Columns(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            });

    //        var exception = Assert.Throws<InvalidOperationException>(() =>
    //            commandBuilder.Columns(user => user.Email));

    //        Assert.That(
    //            exception!.Message,
    //            Is.EqualTo("Property 'Email' is already configured as an INSERT target column."));
    //    }

    //    /// <summary>
    //    /// Validates that INSERT SELECT target column selectors cannot be null.
    //    /// </summary>
    //    [Test]
    //    public void Columns_WhenSelectorIsNull_ShouldThrow()
    //    {
    //        Expression<Func<JoinUser, object>> selector = null!;

    //        Assert.Throws<ArgumentNullException>(() => _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinUser>()
    //            .Columns(selector));
    //    }

    //    /// <summary>
    //    /// Validates that INSERT SELECT target column selectors
    //    /// only reference direct entity properties.
    //    /// </summary>
    //    [Test]
    //    public void Columns_WhenSelectorIsNotDirectProperty_ShouldThrow()
    //    {
    //        var exception = Assert.Throws<ArgumentException>(() => _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinUser>()
    //            .Columns(user => new
    //            {
    //                Value = user.Email!.Length
    //            }));

    //        Assert.That(
    //            exception!.Message,
    //            Does.Contain("The INSERT columns selector must reference direct entity properties."));
    //    }

    //    /// <summary>
    //    /// Validates that INSERT SELECT source aliases cannot contain whitespace-only values.
    //    /// </summary>
    //    [Test]
    //    public void From_WhenAliasIsWhitespace_ShouldThrow()
    //    {
    //        Assert.Throws<ArgumentException>(() => _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinUser>()
    //            .From<JoinUser>(alias: " "));
    //    }

    //    /// <summary>
    //    /// Validates that explicit INSERT SELECT source table names cannot be empty.
    //    /// </summary>
    //    [Test]
    //    public void From_WhenExplicitTableNameIsEmpty_ShouldThrow()
    //    {
    //        Assert.Throws<ArgumentException>(() => _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinUser>()
    //            .From<JoinUser>("", alias: "u"));
    //    }
    //}
}
