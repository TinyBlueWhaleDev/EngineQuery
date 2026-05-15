
namespace TinyBlueWhale.EngineQuery.Tests.Models
{
    public sealed class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public sealed class JoinUser
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public sealed class JoinOrder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Total { get; set; }
    }

    public sealed class JoinOrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int Quantity { get; set; }
    }

    public sealed class OrderSummary
    {
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public int OrderCount { get; set; }
    }

    public sealed class ActiveUser
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    public sealed class ArchivedUser
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
    }

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
