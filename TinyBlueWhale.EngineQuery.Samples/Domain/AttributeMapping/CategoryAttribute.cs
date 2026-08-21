using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TinyBlueWhale.EngineQuery.Samples.Domain.AttributeMapping
{
    [Table("categories")]
    public sealed class CategoryAttribute
    {
        [Column("category_id")]
        public int Id { get; set; }

        [Column("parent_category_id")]
        public int? ParentId { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;
    }
}
