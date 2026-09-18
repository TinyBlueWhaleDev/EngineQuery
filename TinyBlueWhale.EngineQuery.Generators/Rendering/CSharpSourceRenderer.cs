using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;

namespace TinyBlueWhale.EngineQuery.Generators.Rendering
{
    /// <summary>
    /// Provides reusable helpers for rendering C# source code from Roslyn symbols
    /// and values used by EngineQuery source generation.
    /// </summary>
    internal static class CSharpSourceRenderer
    {
        /// <summary>
        /// Gets the symbol display format used by generated EngineQuery source.
        /// </summary>
        internal static readonly SymbolDisplayFormat GeneratedTypeDisplayFormat =
            SymbolDisplayFormat.FullyQualifiedFormat.WithMiscellaneousOptions(
                SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions |
                SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

        /// <summary>
        /// Builds the generic parameter list associated with the specified method.
        /// </summary>
        internal static string BuildGenericParameterList(IMethodSymbol method)
        {
            if (method.TypeParameters.Length == 0)
                return string.Empty;

            return $"<{string.Join(", ", method.TypeParameters.Select(static parameter => parameter.Name))}>";
        }

        /// <summary>
        /// Appends generic constraints declared by the specified method.
        /// </summary>
        internal static void AppendGenericConstraints(StringBuilder source, IMethodSymbol method, string indentation)
        {
            foreach (var typeParameter in method.TypeParameters)
            {
                var constraints = BuildGenericConstraints(typeParameter);

                if (constraints.Count == 0)
                    continue;

                source.Append(indentation);
                source.Append("where ");
                source.Append(typeParameter.Name);
                source.Append(" : ");
                source.AppendLine(string.Join(", ", constraints));
            }
        }

        /// <summary>
        /// Appends generic constraints using the specified type substitutions.
        /// </summary>
        internal static void AppendGenericConstraints(StringBuilder source, IEnumerable<ITypeParameterSymbol> typeParameters, IReadOnlyDictionary<ITypeParameterSymbol, string> substitutions, string indentation)
        {
            foreach (var typeParameter in typeParameters)
            {
                var constraints = BuildGenericConstraints(typeParameter, substitutions);

                if (constraints.Count == 0)
                    continue;

                source.Append(indentation);
                source.Append("where ");
                source.Append(typeParameter.Name);
                source.Append(" : ");
                source.AppendLine(string.Join(", ", constraints));
            }
        }

        /// <summary>
        /// Builds the source representation of a generated required method parameter.
        /// </summary>
        internal static string BuildRequiredParameterDeclaration(IParameterSymbol parameter)
        {
            var modifier = GetRefModifier(parameter.RefKind);
            var type = parameter.Type.ToDisplayString(GeneratedTypeDisplayFormat);

            return $"{modifier}{type} {EscapeIdentifier(parameter.Name)}";
        }

        /// <summary>
        /// Builds the source representation of a generated method parameter.
        /// </summary>
        internal static string BuildParameterDeclaration(IParameterSymbol parameter)
        {
            var modifier = GetRefModifier(parameter.RefKind);
            var type = parameter.Type.ToDisplayString(GeneratedTypeDisplayFormat);
            var defaultValue = BuildDefaultValue(parameter);

            return $"{modifier}{type} {EscapeIdentifier(parameter.Name)}{defaultValue}";
        }

        /// <summary>
        /// Builds a parameter declaration using generic type substitutions.
        /// </summary>
        internal static string BuildParameterDeclaration(IParameterSymbol parameter, IReadOnlyDictionary<ITypeParameterSymbol, string> substitutions)
        {
            var modifier = GetRefModifier(parameter.RefKind);
            var type = RenderType(parameter.Type, substitutions);
            var defaultValue = BuildDefaultValue(parameter);

            return $"{modifier}{type} {EscapeIdentifier(parameter.Name)}{defaultValue}";
        }

        /// <summary>
        /// Renders a type while replacing selected generic type parameters.
        /// </summary>
        internal static string RenderType(ITypeSymbol type, IReadOnlyDictionary<ITypeParameterSymbol, string> substitutions)
        {
            var parts = type.ToDisplayParts(GeneratedTypeDisplayFormat);
            var source = new StringBuilder();

            foreach (var part in parts)
            {
                if (part.Symbol is ITypeParameterSymbol typeParameter &&
                    TryGetTypeSubstitution(typeParameter, substitutions, out var substitution))
                {
                    source.Append(substitution);
                }
                else
                {
                    source.Append(part.ToString());
                }
            }

            return source.ToString();
        }

        /// <summary>
        /// Builds the forwarding invocation argument associated with a method parameter.
        /// </summary>
        internal static string BuildArgument(IParameterSymbol parameter)
        {
            return $"{GetRefModifier(parameter.RefKind)}{EscapeIdentifier(parameter.Name)}";
        }

        /// <summary>
        /// Builds the suffix used to append forwarded arguments after the root builder argument.
        /// </summary>
        internal static string BuildForwardedArgumentSuffix(string arguments)
        {
            return string.IsNullOrWhiteSpace(arguments) ? string.Empty : $", {arguments}";
        }

        /// <summary>
        /// Gets the C# parameter modifier associated with the specified reference kind.
        /// </summary>
        internal static string GetRefModifier(RefKind refKind)
        {
            return refKind switch
            {
                RefKind.Ref => "ref ",
                RefKind.Out => "out ",
                RefKind.In => "in ",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Builds the default value declaration associated with an optional parameter.
        /// </summary>
        internal static string BuildDefaultValue(IParameterSymbol parameter)
        {
            if (!parameter.HasExplicitDefaultValue)
                return string.Empty;

            if (parameter.ExplicitDefaultValue is null)
                return " = null";

            return parameter.ExplicitDefaultValue switch
            {
                bool value => value ? " = true" : " = false",
                string value => $" = \"{EscapeString(value)}\"",
                char value => $" = '{EscapeChar(value)}'",
                float value => $" = {value.ToString(CultureInfo.InvariantCulture)}F",
                double value => $" = {value.ToString(CultureInfo.InvariantCulture)}D",
                decimal value => $" = {value.ToString(CultureInfo.InvariantCulture)}M",
                _ => $" = {Convert.ToString(parameter.ExplicitDefaultValue, CultureInfo.InvariantCulture)}"
            };
        }

        /// <summary>
        /// Escapes a C# identifier when it conflicts with a language keyword.
        /// </summary>
        internal static string EscapeIdentifier(string identifier)
        {
            return Microsoft.CodeAnalysis.CSharp.SyntaxFacts.GetKeywordKind(identifier) != Microsoft.CodeAnalysis.CSharp.SyntaxKind.None
                ? $"@{identifier}"
                : identifier;
        }

        /// <summary>
        /// Gets the generated query engine name associated with the specified profile.
        /// </summary>
        internal static string GetEngineName(INamedTypeSymbol profile)
        {
            return $"{GetProfileBaseName(profile)}QueryEngine";
        }

        /// <summary>
        /// Gets the generated query builder extension class name associated with the specified profile.
        /// </summary>
        internal static string GetExtensionClassName(INamedTypeSymbol profile)
        {
            return $"{GetProfileBaseName(profile)}QueryBuilderExtensions";
        }

        /// <summary>
        /// Builds generic constraints associated with a type parameter.
        /// </summary>
        private static IReadOnlyList<string> BuildGenericConstraints(ITypeParameterSymbol typeParameter)
        {
            var constraints = new List<string>();

            if (typeParameter.HasUnmanagedTypeConstraint)
                constraints.Add("unmanaged");
            else if (typeParameter.HasValueTypeConstraint)
                constraints.Add("struct");
            else if (typeParameter.HasReferenceTypeConstraint)
                constraints.Add(typeParameter.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated ? "class?" : "class");
            else if (typeParameter.HasNotNullConstraint)
                constraints.Add("notnull");

            constraints.AddRange(typeParameter.ConstraintTypes.Select(constraint => constraint.ToDisplayString(GeneratedTypeDisplayFormat)));

            if (typeParameter.HasConstructorConstraint)
                constraints.Add("new()");

            return constraints;
        }

        /// <summary>
        /// Builds generic constraints associated with a type parameter using substitutions.
        /// </summary>
        private static IReadOnlyList<string> BuildGenericConstraints(ITypeParameterSymbol typeParameter, IReadOnlyDictionary<ITypeParameterSymbol, string> substitutions)
        {
            var constraints = new List<string>();

            if (typeParameter.HasUnmanagedTypeConstraint)
                constraints.Add("unmanaged");
            else if (typeParameter.HasValueTypeConstraint)
                constraints.Add("struct");
            else if (typeParameter.HasReferenceTypeConstraint)
                constraints.Add(typeParameter.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated ? "class?" : "class");
            else if (typeParameter.HasNotNullConstraint)
                constraints.Add("notnull");

            constraints.AddRange(typeParameter.ConstraintTypes.Select(constraint => RenderType(constraint, substitutions)));

            if (typeParameter.HasConstructorConstraint)
                constraints.Add("new()");

            return constraints;
        }

        /// <summary>
        /// Attempts to resolve a generic type parameter substitution.
        /// </summary>
        private static bool TryGetTypeSubstitution(ITypeParameterSymbol typeParameter, IReadOnlyDictionary<ITypeParameterSymbol, string> substitutions, out string substitution)
        {
            foreach (var pair in substitutions)
            {
                if (!SymbolEqualityComparer.Default.Equals(pair.Key, typeParameter))
                    continue;

                substitution = pair.Value;
                return true;
            }

            substitution = string.Empty;
            return false;
        }

        /// <summary>
        /// Escapes a string value for inclusion in generated C# source.
        /// </summary>
        private static string EscapeString(string value)
        {
            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
        }

        /// <summary>
        /// Escapes a character value for inclusion in generated C# source.
        /// </summary>
        private static string EscapeChar(char value)
        {
            return value switch
            {
                '\\' => "\\\\",
                '\'' => "\\'",
                '\r' => "\\r",
                '\n' => "\\n",
                '\t' => "\\t",
                _ => value.ToString()
            };
        }

        /// <summary>
        /// Gets the provider profile name without the conventional Profile suffix.
        /// </summary>
        private static string GetProfileBaseName(INamedTypeSymbol profile)
        {
            const string profileSuffix = "Profile";

            var profileName = profile.Name;

            if (profileName.EndsWith(profileSuffix, StringComparison.Ordinal))
                profileName = profileName.Substring(0, profileName.Length - profileSuffix.Length);

            return profileName;
        }
    }
}
