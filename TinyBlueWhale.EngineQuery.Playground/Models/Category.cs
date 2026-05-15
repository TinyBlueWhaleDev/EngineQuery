using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Playground.Models
{
    /// <summary>
    /// Represents a category used by recursive CTE examples.
    /// </summary>
    public sealed class Category
    {
        /// <summary>
        /// Gets or sets the category identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the parent category identifier.
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a recursive category tree projection.
    /// </summary>
    public sealed class CategoryTree
    {
        /// <summary>
        /// Gets or sets the category identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the parent category identifier.
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
