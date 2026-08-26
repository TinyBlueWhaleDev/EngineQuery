using System.Text;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Tests.Models;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Insert
{
    /// <summary>
    /// Validates the public operations exposed by each INSERT command builder stage.
    /// </summary>
    [TestFixture]
    internal sealed class InsertStageTests
    {
        /// <summary>
        /// Validates that the initial INSERT stage exposes value and
        /// INSERT SELECT configuration operations without allowing compilation.
        /// </summary>
        [Test]
        public void InitialStage_ShouldExposeExpectedOperations()
        {
            var methodNames = GetMethodNames(
                typeof(IInsertCommandBuilder<JoinUser>));

            Assert.Multiple(() =>
            {
                Assert.That(methodNames, Does.Contain("Columns"));
                Assert.That(methodNames, Does.Contain("Set"));
                Assert.That(methodNames, Does.Contain("From"));

                Assert.That(methodNames, Does.Not.Contain("Build"));
                Assert.That(methodNames, Does.Not.Contain("WhereIn"));
                Assert.That(methodNames, Does.Not.Contain("WhereNotIn"));
            });
        }

        /// <summary>
        /// Validates that the INSERT VALUES stage exposes additional value
        /// assignments, identity retrieval and command compilation.
        /// </summary>
        [Test]
        public void ValuesStage_ShouldExposeExpectedOperations()
        {
            var methodNames = GetMethodNames(
                typeof(IInsertValuesCommandBuilder<JoinUser>));

            Assert.Multiple(() =>
            {
                Assert.That(methodNames, Does.Contain("Set"));
                Assert.That(methodNames, Does.Contain("ReturnIdentity"));
                Assert.That(methodNames, Does.Contain("Build"));

                Assert.That(methodNames, Does.Not.Contain("Columns"));
                Assert.That(methodNames, Does.Not.Contain("From"));
                Assert.That(methodNames, Does.Not.Contain("Select"));
                Assert.That(methodNames, Does.Not.Contain("WhereIn"));
                Assert.That(methodNames, Does.Not.Contain("WhereNotIn"));
            });
        }

        /// <summary>
        /// Validates that the INSERT SELECT stage exposes query composition
        /// and command compilation without allowing INSERT VALUES operations.
        /// </summary>
        [Test]
        public void SelectStage_ShouldExposeExpectedOperations()
        {
            var methodNames = GetMethodNames(
                typeof(IInsertSelectCommandBuilder<JoinUser>));

            Assert.Multiple(() =>
            {
                Assert.That(methodNames, Does.Contain("Select"));
                Assert.That(methodNames, Does.Contain("Where"));
                Assert.That(methodNames, Does.Contain("WhereIn"));
                Assert.That(methodNames, Does.Contain("WhereNotIn"));
                Assert.That(methodNames, Does.Contain("Build"));

                Assert.That(methodNames, Does.Not.Contain("Columns"));
                Assert.That(methodNames, Does.Not.Contain("Set"));
                Assert.That(methodNames, Does.Not.Contain("From"));
                Assert.That(methodNames, Does.Not.Contain("ReturnIdentity"));
            });
        }

        /// <summary>
        /// Resolves the public method names exposed by an INSERT builder stage,
        /// including methods inherited from parent interfaces.
        /// </summary>
        /// <param name="interfaceType">
        /// INSERT builder interface being inspected.
        /// </param>
        /// <returns>
        /// Distinct method names exposed by the interface hierarchy.
        /// </returns>
        private static IReadOnlyCollection<string> GetMethodNames(Type interfaceType)
        {
            return
            [
                .. interfaceType
                    .GetInterfaces()
                    .Append(interfaceType)
                    .SelectMany(type => type.GetMethods())
                    .Select(method => method.Name)
                    .Distinct()
            ];
        }
    }
}
