using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Sql.Clauses;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.Sql.Composition
{
    /// <summary>
    /// Defines provider-specific options used while creating a query script builder.
    /// </summary>
    /// <remarks>
    /// These options allow providers to override only the SQL clause builders that require
    /// provider-specific behavior while reusing the default query compilation pipeline.
    /// </remarks>
    public sealed class QueryScriptBuilderOptions
    {
        /// <summary>
        /// Gets or initializes the optional factory used to create the APPLY clause builder.
        /// </summary>
        /// <remarks>
        /// When this value is <see langword="null"/>, the default <see cref="ApplyClauseBuilder"/> is used.
        /// </remarks>
        public Func<SubqueryCompiler, ApplyClauseBuilder>? ApplyClauseBuilderFactory { get; init; }

        /// <summary>
        /// Gets or initializes the optional factory used to create the CTE clause builder.
        /// </summary>
        /// <remarks>
        /// When this value is <see langword="null"/>, the default <see cref="CteClauseBuilder"/> is used.
        /// </remarks>
        public Func<SubqueryCompiler, CteClauseBuilder>? CteClauseBuilderFactory { get; init; }
    }
}
