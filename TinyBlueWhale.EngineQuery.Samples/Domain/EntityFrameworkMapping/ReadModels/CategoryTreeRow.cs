using System.ComponentModel.DataAnnotations.Schema;

namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    [Table("category_tree")]
    public sealed class CategoryTreeRow
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("ParentId")]
        public int? ParentId { get; set; }

        [Column("Name")]
        public string Name { get; set; } = string.Empty;
    }
}
