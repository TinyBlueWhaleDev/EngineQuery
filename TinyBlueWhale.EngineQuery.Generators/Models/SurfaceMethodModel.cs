using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace TinyBlueWhale.EngineQuery.Generators.Models
{
    /// <summary>
    /// Represents a method declared by a specific feature surface.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SurfaceMethodModel"/> class.
    /// </remarks>
    /// <param name="surface">
    /// Feature surface declaring the method.
    /// </param>
    /// <param name="method">
    /// Method declared by the feature surface.
    /// </param>
    internal sealed class SurfaceMethodModel(INamedTypeSymbol surface, IMethodSymbol method)
    {
        /// <summary>
        /// Gets the feature surface declaring the method.
        /// </summary>
        public INamedTypeSymbol Surface { get; } = surface ?? throw new ArgumentNullException(nameof(surface));

        /// <summary>
        /// Gets the method declared by the feature surface.
        /// </summary>
        public IMethodSymbol Method { get; } = method ?? throw new ArgumentNullException(nameof(method));
    }
}
