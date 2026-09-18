using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace TinyBlueWhale.EngineQuery.Generators.Models
{
    /// <summary>
    /// Represents a provider profile together with compatible root and
    /// composition feature surfaces.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ProfileSurfaceModel"/> class.
    /// </remarks>
    /// <param name="profile">
    /// Database provider profile represented by the model.
    /// </param>
    /// <param name="rootSurfaces">
    /// Root query feature surfaces compatible with the profile.
    /// </param>
    /// <param name="compositionSurfaces">
    /// Composition query feature surfaces compatible with the profile.
    /// </param>
    /// <param name="isLocal">
    /// Indicates whether the profile is declared by the current compilation.
    /// </param>
    internal sealed class ProfileSurfaceModel(INamedTypeSymbol profile, IReadOnlyList<INamedTypeSymbol> rootSurfaces, IReadOnlyList<INamedTypeSymbol> compositionSurfaces, bool isLocal)
    {
        /// <summary>
        /// Gets the database provider profile represented by the model.
        /// </summary>
        public INamedTypeSymbol Profile { get; } = profile ?? throw new ArgumentNullException(nameof(profile));

        /// <summary>
        /// Gets root query feature surfaces compatible with the profile.
        /// </summary>
        public IReadOnlyList<INamedTypeSymbol> RootSurfaces { get; } = rootSurfaces ?? throw new ArgumentNullException(nameof(rootSurfaces));

        /// <summary>
        /// Gets composition query feature surfaces compatible with the profile.
        /// </summary>
        public IReadOnlyList<INamedTypeSymbol> CompositionSurfaces { get; } = compositionSurfaces ?? throw new ArgumentNullException(nameof(compositionSurfaces));

        /// <summary>
        /// Gets whether the profile is declared by the current compilation.
        /// </summary>
        public bool IsLocal { get; } = isLocal;
    }
}
