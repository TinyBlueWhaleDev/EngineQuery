using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.Parameters;
using TinyBlueWhale.EngineQuery.Sql.Helpers;

namespace TinyBlueWhale.EngineQuery.Tests.Helpers
{
    [TestFixture]
    public sealed class SqlParameterRewriterTests
    {
        [Test]
        public void Rewrite_WhenRewrittenNameCollidesWithPendingSourceParameter_RewritesAtomically()
        {
            // Arrange
            var targetParameters = new QueryParameterCollection();

            for (var index = 0; index < 6; index++)
                targetParameters.Add($"target-{index}");

            var sourceParameters = Enumerable
                .Range(0, 7)
                .Select(index => new QuerySqlParameter
                {
                    Name = $"@p{index}",
                    Value = $"source-{index}"
                })
                .ToArray();

            const string commandText =
                "WHERE A = @p0 AND B = @p1 AND C = @p6";

            // Act
            var result = SqlParameterRewriter.Rewrite(
                commandText,
                sourceParameters,
                targetParameters);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(
                    result,
                    Is.EqualTo("WHERE A = @p6 AND B = @p7 AND C = @p12"));

                Assert.That(
                    targetParameters.Parameters,
                    Has.Count.EqualTo(13));

                Assert.That(
                    targetParameters.Parameters[6].Name,
                    Is.EqualTo("@p6"));

                Assert.That(
                    targetParameters.Parameters[6].Value,
                    Is.EqualTo("source-0"));

                Assert.That(
                    targetParameters.Parameters[7].Name,
                    Is.EqualTo("@p7"));

                Assert.That(
                    targetParameters.Parameters[7].Value,
                    Is.EqualTo("source-1"));

                Assert.That(
                    targetParameters.Parameters[12].Name,
                    Is.EqualTo("@p12"));

                Assert.That(
                    targetParameters.Parameters[12].Value,
                    Is.EqualTo("source-6"));
            });
        }

        [Test]
        public void Rewrite_WhenParameterNamesSharePrefix_DoesNotReplacePartialNames()
        {
            // Arrange
            var targetParameters = new QueryParameterCollection();

            var sourceParameters = new[]
            {
                new QuerySqlParameter
                {
                    Name = "@p1",
                    Value = "one"
                },
                new QuerySqlParameter
                {
                    Name = "@p10",
                    Value = "ten"
                }
            };

            const string commandText =
                "WHERE A = @p1 AND B = @p10";

            // Act
            var result = SqlParameterRewriter.Rewrite(
                commandText,
                sourceParameters,
                targetParameters);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(
                    result,
                    Is.EqualTo("WHERE A = @p0 AND B = @p1"));

                Assert.That(
                    targetParameters.Parameters,
                    Has.Count.EqualTo(2));

                Assert.That(
                    targetParameters.Parameters[0].Value,
                    Is.EqualTo("one"));

                Assert.That(
                    targetParameters.Parameters[1].Value,
                    Is.EqualTo("ten"));
            });
        }

        [Test]
        public void Rewrite_WhenSourceParametersAreEmpty_ReturnsCommandTextUnchanged()
        {
            // Arrange
            var targetParameters = new QueryParameterCollection();
            const string commandText = "SELECT * FROM Users";

            // Act
            var result = SqlParameterRewriter.Rewrite(
                commandText,
                [],
                targetParameters);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(commandText));
                Assert.That(targetParameters.Parameters, Is.Empty);
            });
        }

        [Test]
        public void Rewrite_WhenSameParameterAppearsMultipleTimes_RewritesAllOccurrences()
        {
            // Arrange
            var targetParameters = new QueryParameterCollection();

            var sourceParameters = new[]
            {
                new QuerySqlParameter
                {
                    Name = "@p0",
                    Value = 10
                }
            };

            const string commandText =
                "WHERE A = @p0 OR B = @p0 OR C = @p0";

            // Act
            var result = SqlParameterRewriter.Rewrite(
                commandText,
                sourceParameters,
                targetParameters);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(
                    result,
                    Is.EqualTo("WHERE A = @p0 OR B = @p0 OR C = @p0"));

                Assert.That(
                    targetParameters.Parameters,
                    Has.Count.EqualTo(1));

                Assert.That(
                    targetParameters.Parameters[0].Value,
                    Is.EqualTo(10));
            });
        }

        [Test]
        public void Rewrite_WhenTargetAlreadyContainsParameters_PreservesSourceParameterOrderAndValues()
        {
            // Arrange
            var targetParameters = new QueryParameterCollection();

            targetParameters.Add("parent-0");
            targetParameters.Add("parent-1");

            var sourceParameters = new[]
            {
                new QuerySqlParameter
                {
                    Name = "@p0",
                    Value = "source-0"
                },
                new QuerySqlParameter
                {
                    Name = "@p1",
                    Value = "source-1"
                },
                new QuerySqlParameter
                {
                    Name = "@p2",
                    Value = "source-2"
                }
            };

            const string commandText =
                "WHERE A = @p0 AND B = @p1 AND C = @p2";

            // Act
            var result = SqlParameterRewriter.Rewrite(
                commandText,
                sourceParameters,
                targetParameters);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(
                    result,
                    Is.EqualTo("WHERE A = @p2 AND B = @p3 AND C = @p4"));

                Assert.That(
                    targetParameters.Parameters.Select(parameter => parameter.Value),
                    Is.EqualTo(new object?[]
                    {
                        "parent-0",
                        "parent-1",
                        "source-0",
                        "source-1",
                        "source-2"
                    }));
            });
        }

        [Test]
        public void Rewrite_WhenCommandTextDoesNotReferenceSourceParameter_StillAddsRewrittenParameter()
        {
            // Arrange
            var targetParameters = new QueryParameterCollection();

            var sourceParameters = new[]
            {
                new QuerySqlParameter
                {
                    Name = "@p0",
                    Value = 42
                }
            };

            const string commandText = "SELECT 1";

            // Act
            var result = SqlParameterRewriter.Rewrite(
                commandText,
                sourceParameters,
                targetParameters);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(commandText));
                Assert.That(targetParameters.Parameters, Has.Count.EqualTo(1));
                Assert.That(targetParameters.Parameters[0].Name, Is.EqualTo("@p0"));
                Assert.That(targetParameters.Parameters[0].Value, Is.EqualTo(42));
            });
        }

        [Test]
        public void Rewrite_WhenSourceParametersAreNull_ThrowsArgumentNullException()
        {
            // Arrange
            var targetParameters = new QueryParameterCollection();

            // Act
            var exception = Assert.Throws<ArgumentNullException>(
                () => SqlParameterRewriter.Rewrite(
                    "SELECT 1",
                    null!,
                    targetParameters));

            // Assert
            Assert.That(
                exception!.ParamName,
                Is.EqualTo("sourceParameters"));
        }

        [Test]
        public void Rewrite_WhenTargetParametersAreNull_ThrowsArgumentNullException()
        {
            // Arrange
            var sourceParameters = Array.Empty<QuerySqlParameter>();

            // Act
            var exception = Assert.Throws<ArgumentNullException>(
                () => SqlParameterRewriter.Rewrite(
                    "SELECT 1",
                    sourceParameters,
                    null!));

            // Assert
            Assert.That(
                exception!.ParamName,
                Is.EqualTo("targetParameters"));
        }
    }
}
