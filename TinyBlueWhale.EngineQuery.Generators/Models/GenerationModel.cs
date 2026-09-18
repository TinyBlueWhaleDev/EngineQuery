using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;

namespace TinyBlueWhale.EngineQuery.Generators.Models
{
    /// <summary>
    /// Represents the complete generation state associated with a compilation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="GenerationModel"/> class.
    /// </remarks>
    /// <param name="compilation">
    /// Compilation associated with the current generation pipeline.
    /// </param>
    /// <param name="profiles">
    /// Discovered provider profile surface models.
    /// </param>
    /// <param name="profileContract">
    /// Database provider profile contract used during structural feature discovery.
    /// </param>
    /// <param name="supportsQueryEngineGeneration">
    /// Indicates whether the current compilation supports query engine and
    /// dependency injection generation.
    /// </param>
    internal sealed class GenerationModel(Compilation compilation, ImmutableArray<ProfileSurfaceModel> profiles, INamedTypeSymbol? profileContract, bool supportsQueryEngineGeneration)
    {
        /// <summary>
        /// Gets the compilation associated with the current generation pipeline.
        /// </summary>
        public Compilation Compilation { get; } = compilation;

        /// <summary>
        /// Gets the discovered provider profile surface models.
        /// </summary>
        public ImmutableArray<ProfileSurfaceModel> Profiles { get; } = profiles;

        /// <summary>
        /// Gets the database provider profile contract used during structural
        /// feature discovery.
        /// </summary>
        public INamedTypeSymbol? ProfileContract { get; } = profileContract;

        /// <summary>
        /// Gets whether the current compilation supports query engine and
        /// dependency injection generation.
        /// </summary>
        public bool SupportsQueryEngineGeneration { get; } = supportsQueryEngineGeneration;
    }
}
