using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Tests.Models;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{
    /// <summary>
    /// Validates the operations exposed by each INSERT command builder stage.
    /// </summary>
    [TestFixture]
    public sealed class InsertCommandBuilderStageTests
    {
        #region Tests

        [Test]
        public void Initial_Stage_Should_Expose_Columns_Set_And_From()
        {
            // Arrange
            IReadOnlyCollection<string> methodNames = GetMethodNames(
                typeof(IInsertCommandBuilder<JoinUser>));

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.That(methodNames, Does.Contain("Columns"));
                Assert.That(methodNames, Does.Contain("Set"));
                Assert.That(methodNames, Does.Contain("From"));
                Assert.That(methodNames, Does.Not.Contain("Build"));
            });
        }

        [Test]
        public void Values_Stage_Should_Expose_Set_ReturnIdentity_And_Build()
        {
            // Arrange
            IReadOnlyCollection<string> methodNames = GetMethodNames(
                typeof(IInsertValuesCommandBuilder<JoinUser>));

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.That(methodNames, Does.Contain("Set"));
                Assert.That(methodNames, Does.Contain("ReturnIdentity"));
                Assert.That(methodNames, Does.Contain("Build"));
                Assert.That(methodNames, Does.Not.Contain("Columns"));
                Assert.That(methodNames, Does.Not.Contain("From"));
                Assert.That(methodNames, Does.Not.Contain("Select"));
            });
        }


        [Test]
        public void Select_Stage_Should_Expose_Query_Composition_And_Build()
        {
            // Arrange
            IReadOnlyCollection<string> methodNames = GetMethodNames(
                typeof(IInsertSelectCommandBuilder<JoinUser>));

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.That(methodNames, Does.Contain("Select"));
                Assert.That(methodNames, Does.Contain("Where"));
                Assert.That(methodNames, Does.Contain("Build"));
                Assert.That(methodNames, Does.Not.Contain("Columns"));
                Assert.That(methodNames, Does.Not.Contain("Set"));
                Assert.That(methodNames, Does.Not.Contain("From"));
            });
        }

        #endregion

        #region Private Methods

        private static IReadOnlyCollection<string> GetMethodNames(Type interfaceType)
        {
            return [.. interfaceType
                .GetInterfaces()
                .Append(interfaceType)
                .SelectMany(type => type.GetMethods())
                .Select(method => method.Name)
                .Distinct()];
        }

        #endregion
    }
}
