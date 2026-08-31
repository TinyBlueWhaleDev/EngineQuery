using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Attributes
{
    /// <summary>
    /// Associates a query feature contract with the generic query builder surface
    /// exposed when the feature is supported by a database provider profile.
    /// </summary>
    /// <remarks>
    /// The specified surface type must be an open generic interface receiving the
    /// database provider profile as its generic argument.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class QueryFeatureSurfaceAttribute(Type surfaceType) : Attribute
    {
        /// <summary>
        /// Gets the open generic query builder surface associated with the feature.
        /// </summary>
        public Type SurfaceType { get; } = surfaceType ?? throw new ArgumentNullException(nameof(surfaceType));
    }
}
