using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Tests.Models;
using TinyBlueWhale.EngineQuery.Tests.Providers;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Insert
{
    /// <summary>
    /// Validates provider-independent INSERT identity retrieval behavior.
    /// </summary>
    //[TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    //internal sealed class InsertIdentityTests(IQueryTestProvider provider)
    //{
    //    private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    //    /// <summary>
    //    /// Validates provider-specific identity retrieval generation.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenIdentityRetrievalIsConfigured_ShouldGenerateIdentitySql()
    //    {
    //        var commandBuilder = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<JoinUser>()
    //            .Set(user => user.Email, "admin@test.com");

    //        var query = ConfigureIdentityRetrieval(commandBuilder)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("INSERT"));
    //            Assert.That(query.CommandText, Does.Contain("users"));
    //            Assert.That(query.CommandText, Does.Contain("email"));

    //            Assert.That(query.Parameters, Has.Count.EqualTo(1));
    //            Assert.That(query.Parameters[0].Value, Is.EqualTo("admin@test.com"));

    //            AssertIdentitySyntax(query.CommandText);
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that INSERT identity retrieval cannot be configured more than once.
    //    /// </summary>
    //    [Test]
    //    public void ReturnIdentity_WhenAlreadyConfigured_ShouldThrow()
    //    {
    //        var commandBuilder = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<User>()
    //            .Set(user => user.Email, "admin@test.com")
    //            .ReturnIdentity();

    //        var exception = Assert.Throws<InvalidOperationException>(() =>
    //            commandBuilder.ReturnIdentity());

    //        Assert.That(exception!.Message, Is.EqualTo("Identity retrieval is already configured for the current INSERT command."));
    //    }

    //    /// <summary>
    //    /// Validates that an INSERT identity selector cannot be null.
    //    /// </summary>
    //    [Test]
    //    public void ReturnIdentity_WhenSelectorIsNull_ShouldThrow()
    //    {
    //        Expression<Func<User, int>> selector = null!;

    //        Assert.Throws<ArgumentNullException>(() => _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<User>()
    //            .Set(user => user.Email, "admin@test.com")
    //            .ReturnIdentity(selector));
    //    }

    //    /// <summary>
    //    /// Validates that an INSERT identity selector must reference a direct entity property.
    //    /// </summary>
    //    [Test]
    //    public void ReturnIdentity_WhenSelectorIsNotDirectProperty_ShouldThrow()
    //    {
    //        var exception = Assert.Throws<ArgumentException>(() => _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<User>()
    //            .Set(user => user.Email, "admin@test.com")
    //            .ReturnIdentity(user => user.Email.Length));

    //        Assert.That(exception!.Message, Does.Contain("The INSERT identity selector must reference a direct entity property. (Parameter 'identitySelector')"));
    //    }

    //    /// <summary>
    //    /// Configures identity retrieval using the syntax required by the current provider.
    //    /// </summary>
    //    private IInsertValuesCommandBuilder<JoinUser> ConfigureIdentityRetrieval(
    //        IInsertValuesCommandBuilder<JoinUser> commandBuilder)
    //    {
    //        return _provider.ProviderName switch
    //        {
    //            "PostgreSql" => commandBuilder.ReturnIdentity(user => user.Id),
    //            "SqlServer" => commandBuilder.ReturnIdentity(),
    //            "MySql" => commandBuilder.ReturnIdentity(),
    //            _ => throw new NotSupportedException(
    //                $"Provider '{_provider.ProviderName}' is not supported by INSERT identity tests.")
    //        };
    //    }

    //    /// <summary>
    //    /// Validates that the generated command contains the identity retrieval
    //    /// syntax required by the current provider.
    //    /// </summary>
    //    private void AssertIdentitySyntax(string commandText)
    //    {
    //        switch (_provider.ProviderName)
    //        {
    //            case "PostgreSql":
    //                Assert.That(commandText, Does.Contain("RETURNING"));
    //                break;

    //            case "SqlServer":
    //            case "MySql":
    //                Assert.That(commandText, Does.Contain("IDENTITY").IgnoreCase.Or.Contain("LAST_INSERT_ID").IgnoreCase);
    //                break;

    //            default:
    //                Assert.Fail($"Provider '{_provider.ProviderName}' is not supported by INSERT identity tests.");
    //                break;
    //        }
    //    }
    //}
}
