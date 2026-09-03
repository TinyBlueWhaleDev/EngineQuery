namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Aliases
{
    ///// <summary>
    ///// Validates provider-independent query source alias behavior.
    ///// </summary>
    //[TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    //internal sealed class AliasTests(IQueryTestProvider provider)
    //{
    //    private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    //    /// <summary>
    //    /// Validates that a common table expression source does not generate
    //    /// an unnecessary alias when one is not explicitly configured.
    //    /// </summary>
    //    [Test]
    //    public void FromCte_WhenAliasIsNotProvided_ShouldNotGenerateAlias()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .With<OrderSummary, JoinOrder>(
    //                "order_summary",
    //                cte => cte
    //                    .From<JoinOrder>()
    //                    .Select(order => new
    //                    {
    //                        order.UserId
    //                    }))
    //            .FromCte<OrderSummary>("order_summary")
    //            .Select(summary => summary.UserId)
    //            .Build();

    //        var commandText = NormalizeSql(query.CommandText);

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(commandText, Does.Contain("FROM order_summary"));
    //            Assert.That(commandText, Does.Not.Contain("FROM order_summary AS"));
    //            Assert.That(commandText, Does.Contain("UserId"));
    //            Assert.That(commandText, Does.Not.Contain("order_summary.UserId"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that an explicitly configured common table expression alias
    //    /// is preserved and used to qualify generated column references.
    //    /// </summary>
    //    [Test]
    //    public void FromCte_WhenAliasIsProvided_ShouldUseExplicitAlias()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .With<OrderSummary, JoinOrder>(
    //                "order_summary",
    //                cte => cte
    //                    .From<JoinOrder>()
    //                    .Select(order => new
    //                    {
    //                        order.UserId
    //                    }))
    //            .FromCte<OrderSummary>(
    //                "order_summary",
    //                alias: "os")
    //            .Select(summary => summary.UserId)
    //            .Build();

    //        var commandText = NormalizeSql(query.CommandText);

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(commandText, Does.Contain("FROM order_summary AS os"));
    //            Assert.That(commandText, Does.Contain("os.UserId"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that a single query source does not generate
    //    /// an unnecessary alias.
    //    /// </summary>
    //    [Test]
    //    public void From_WhenAliasIsNotProvided_ShouldNotGenerateAlias()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<User>()
    //            .Select(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .Where(user => user.Id == 1)
    //            .Build();

    //        var commandText = NormalizeSql(query.CommandText);

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(commandText, Does.Contain("FROM Users"));
    //            Assert.That(commandText, Does.Not.Contain("FROM Users AS"));
    //            Assert.That(commandText, Does.Contain("Id"));
    //            Assert.That(commandText, Does.Contain("Email"));
    //            Assert.That(commandText, Does.Not.Contain("t0."));
    //            Assert.That(commandText, Does.Not.Contain("Users.Id"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that an explicitly configured alias is preserved
    //    /// and used to qualify generated column references.
    //    /// </summary>
    //    [Test]
    //    public void From_WhenAliasIsProvided_ShouldUseExplicitAlias()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<User>(alias: "u")
    //            .Select(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .Where(user => user.Id == 1)
    //            .Build();

    //        var commandText = NormalizeSql(query.CommandText);

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(commandText, Does.Contain("FROM Users AS u"));
    //            Assert.That(commandText, Does.Contain("u.Id"));
    //            Assert.That(commandText, Does.Contain("u.Email"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that deterministic aliases are generated when multiple
    //    /// query sources require column qualification.
    //    /// </summary>
    //    [Test]
    //    public void InnerJoin_WhenAliasesAreNotProvided_ShouldGenerateDeterministicAliases()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinUser>()
    //            .InnerJoin<JoinUser, JoinOrder>(
    //                alias: null,
    //                on: (user, order) => user.Id == order.UserId)
    //            .Select<JoinUser>(user => user.Id)
    //            .Select<JoinOrder>(order => order.Id)
    //            .Build();

    //        var commandText = NormalizeSql(query.CommandText);

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(commandText, Does.Contain("FROM users AS t0"));
    //            Assert.That(commandText, Does.Contain("INNER JOIN orders AS t1"));
    //            Assert.That(commandText, Does.Contain("t0.user_id"));
    //            Assert.That(commandText, Does.Contain("t1.order_id"));
    //            Assert.That(commandText, Does.Contain("t0.user_id = t1.user_id"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that explicitly configured aliases are preserved when
    //    /// multiple query sources participate in the query.
    //    /// </summary>
    //    [Test]
    //    public void InnerJoin_WhenAliasesAreProvided_ShouldPreserveExplicitAliases()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinUser>(alias: "u")
    //            .InnerJoin<JoinUser, JoinOrder>(
    //                alias: "o",
    //                on: (user, order) => user.Id == order.UserId)
    //            .Select<JoinUser>(user => user.Id)
    //            .Select<JoinOrder>(order => order.Id)
    //            .Build();

    //        var commandText = NormalizeSql(query.CommandText);

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(commandText, Does.Contain("FROM users AS u"));
    //            Assert.That(commandText, Does.Contain("INNER JOIN orders AS o"));
    //            Assert.That(commandText, Does.Contain("u.user_id"));
    //            Assert.That(commandText, Does.Contain("o.order_id"));
    //            Assert.That(commandText, Does.Contain("u.user_id = o.user_id"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that a root source receives a deterministic alias when
    //    /// a correlated EXISTS subquery requires outer source qualification.
    //    /// </summary>
    //    [Test]
    //    public void WhereExists_WhenRootAliasIsNotProvided_ShouldGenerateDeterministicAlias()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinUser>()
    //            .Select<JoinUser>(user => user.Id)
    //            .WhereExists<JoinUser, JoinOrder>(
    //                alias: "o",
    //                subquery => subquery
    //                    .WhereComputed<JoinOrder, JoinUser>((order, user) =>
    //                        order.UserId == user.Id))
    //            .Build();

    //        var commandText = NormalizeSql(query.CommandText);

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(commandText, Does.Contain("FROM users AS t0"));
    //            Assert.That(commandText, Does.Contain("EXISTS"));
    //            Assert.That(commandText, Does.Contain("FROM orders AS o"));
    //            Assert.That(commandText, Does.Contain("o.user_id = t0.user_id"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that a root source receives a deterministic alias when
    //    /// a correlated IN subquery requires outer source qualification.
    //    /// </summary>
    //    [Test]
    //    public void WhereInSubquery_WhenRootAliasIsNotProvided_ShouldGenerateDeterministicAlias()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinUser>()
    //            .WhereIn<JoinUser, JoinOrder>(
    //                user => user.Id,
    //                alias: "o",
    //                subquery => subquery
    //                    .Select<JoinOrder>(order => new
    //                    {
    //                        order.UserId
    //                    })
    //                    .WhereComputed<JoinOrder, JoinUser>((order, user) =>
    //                        order.UserId == user.Id))
    //            .Build();

    //        var commandText = NormalizeSql(query.CommandText);

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(commandText, Does.Contain("FROM users AS t0"));
    //            Assert.That(commandText, Does.Contain("t0.user_id IN"));
    //            Assert.That(commandText, Does.Contain("FROM orders AS o"));
    //            Assert.That(commandText, Does.Contain("o.user_id = t0.user_id"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that a root source receives a deterministic alias when
    //    /// a correlated APPLY source requires outer source qualification.
    //    /// </summary>
    //    [Test]
    //    public void CrossApply_WhenRootAliasIsNotProvided_ShouldGenerateDeterministicAlias()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinUser>()
    //            .Select<JoinUser>(user => user.Id)
    //            .CrossApply<JoinUser, JoinOrder>(
    //                alias: "latest_order",
    //                apply => apply
    //                    .Select<JoinOrder>(order => new
    //                    {
    //                        order.Id,
    //                        order.UserId
    //                    })
    //                    .WhereComputed<JoinOrder, JoinUser>((order, user) =>
    //                        order.UserId == user.Id))
    //            .Build();

    //        var commandText = NormalizeSql(query.CommandText);

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(commandText, Does.Contain("FROM users AS t0"));
    //            Assert.That(commandText, Does.Contain("latest_order"));
    //            Assert.That(commandText, Does.Contain("orders"));
    //            Assert.That(commandText, Does.Contain("t0.user_id"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that a simple UPDATE command does not qualify columns
    //    /// with an unnecessary table alias.
    //    /// </summary>
    //    [Test]
    //    public void Update_WhenAliasIsNotRequired_ShouldNotQualifyColumns()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .Update<User>()
    //            .Set(user => user.Email, "updated@test.com")
    //            .Where(user => user.Id == 1)
    //            .Build();

    //        var commandText = NormalizeSql(query.CommandText);

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(commandText, Does.Contain("UPDATE Users"));
    //            Assert.That(commandText, Does.Contain("SET Email = @p0"));
    //            Assert.That(commandText, Does.Contain("WHERE"));
    //            Assert.That(commandText, Does.Contain("Id = @p1"));

    //            Assert.That(commandText, Does.Not.Contain("Users.Email"));
    //            Assert.That(commandText, Does.Not.Contain("Users.Id"));
    //            Assert.That(commandText, Does.Not.Contain("t0."));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that a simple DELETE command does not qualify columns
    //    /// with an unnecessary table alias.
    //    /// </summary>
    //    [Test]
    //    public void Delete_WhenAliasIsNotRequired_ShouldNotQualifyColumns()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .DeleteFrom<User>()
    //            .Where(user => user.Id == 1)
    //            .Build();

    //        var commandText = NormalizeSql(query.CommandText);

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(commandText, Does.Contain("DELETE"));
    //            Assert.That(commandText, Does.Contain("FROM Users"));
    //            Assert.That(commandText, Does.Contain("WHERE"));
    //            Assert.That(commandText, Does.Contain("Id = @p0"));

    //            Assert.That(commandText, Does.Not.Contain("Users.Id"));
    //            Assert.That(commandText, Does.Not.Contain("t0."));
    //        });
    //    }

    //    /// <summary>
    //    /// Removes provider-specific identifier delimiters from generated SQL
    //    /// so alias behavior can be validated consistently across providers.
    //    /// </summary>
    //    /// <param name="commandText">
    //    /// Generated SQL command text.
    //    /// </param>
    //    /// <returns>
    //    /// SQL command text without provider-specific identifier delimiters.
    //    /// </returns>
    //    private static string NormalizeSql(string commandText)
    //    {
    //        return commandText
    //            .Replace("[", string.Empty, StringComparison.Ordinal)
    //            .Replace("]", string.Empty, StringComparison.Ordinal)
    //            .Replace("\"", string.Empty, StringComparison.Ordinal)
    //            .Replace("`", string.Empty, StringComparison.Ordinal);
    //    }
    //}
}
