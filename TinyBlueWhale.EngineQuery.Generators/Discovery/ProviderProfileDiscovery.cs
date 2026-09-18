using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;

namespace TinyBlueWhale.EngineQuery.Generators.Discovery
{
    /// <summary>
    /// Discovers concrete EngineQuery database provider profiles available through
    /// the current compilation and referenced EngineQuery assemblies.
    /// </summary>
    internal static class ProviderProfileDiscovery
    {
        /// <summary>
        /// Discovers concrete database provider profiles available through the current
        /// compilation and referenced EngineQuery assemblies.
        /// </summary>
        /// <param name="compilation">
        /// Compilation used to inspect local and referenced assemblies.
        /// </param>
        /// <param name="profileContract">
        /// Database provider profile contract that discovered profiles must implement.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel discovery.
        /// </param>
        /// <returns>
        /// The distinct concrete provider profiles ordered by their fully qualified
        /// display representation.
        /// </returns>
        internal static ImmutableArray<INamedTypeSymbol> Discover(Compilation compilation, INamedTypeSymbol profileContract, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var profiles = ImmutableArray.CreateBuilder<INamedTypeSymbol>();

            CollectProfiles(compilation.Assembly.GlobalNamespace, profileContract, profiles, cancellationToken);

            foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!assembly.Name.StartsWith("TinyBlueWhale.EngineQuery", StringComparison.Ordinal))
                    continue;

                CollectProfiles(assembly.GlobalNamespace, profileContract, profiles, cancellationToken);
            }

            return [.. profiles
                .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
                .OrderBy(static profile => profile.ToDisplayString(), StringComparer.Ordinal)];
        }

        /// <summary>
        /// Traverses the specified namespace recursively and collects concrete classes
        /// implementing the database provider profile contract.
        /// </summary>
        private static void CollectProfiles(INamespaceSymbol namespaceSymbol, INamedTypeSymbol profileContract, ImmutableArray<INamedTypeSymbol>.Builder profiles, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var type in namespaceSymbol.GetTypeMembers())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (IsConcreteProfile(type, profileContract))
                    profiles.Add(type);

                CollectNestedProfiles(type, profileContract, profiles, cancellationToken);
            }

            foreach (var nestedNamespace in namespaceSymbol.GetNamespaceMembers())
                CollectProfiles(nestedNamespace, profileContract, profiles, cancellationToken);
        }

        /// <summary>
        /// Traverses nested types and collects concrete database provider profiles.
        /// </summary>
        private static void CollectNestedProfiles(INamedTypeSymbol containingType, INamedTypeSymbol profileContract, ImmutableArray<INamedTypeSymbol>.Builder profiles, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var nestedType in containingType.GetTypeMembers())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (IsConcreteProfile(nestedType, profileContract))
                    profiles.Add(nestedType);

                CollectNestedProfiles(nestedType, profileContract, profiles, cancellationToken);
            }
        }

        /// <summary>
        /// Determines whether the specified type represents a concrete database
        /// provider profile.
        /// </summary>
        private static bool IsConcreteProfile(INamedTypeSymbol type, INamedTypeSymbol profileContract)
        {
            if (type.TypeKind != TypeKind.Class || type.IsAbstract)
                return false;

            return type.AllInterfaces.Any(implementedInterface => SymbolEqualityComparer.Default.Equals(implementedInterface, profileContract));
        }
    }
}
