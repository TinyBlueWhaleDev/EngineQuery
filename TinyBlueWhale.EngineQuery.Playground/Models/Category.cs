namespace TinyBlueWhale.EngineQuery.Playground.Models
{   
    public sealed class Category
    { 
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public sealed class CategoryTree
    {    
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
