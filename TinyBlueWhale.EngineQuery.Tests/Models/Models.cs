
using System.ComponentModel.DataAnnotations.Schema;

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
        public int TenantId { get; set; }
    }

    public sealed class JoinOrder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Total { get; set; }
        public int TenantId { get; set; }
        public int ApproverUserId { get; set; }
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

    public sealed class EfUser
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }

    public sealed class EfSchemaUser
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;
    }

    public sealed class EfUserWithShadowProperty
    {
        public int Id { get; set; }
    }

    public sealed class EfUserWithIgnoredProperty
    {
        public int Id { get; set; }

        public string IgnoredValue { get; set; } = string.Empty;
    }

    public sealed class UnmappedEntity
    {
        public int Id { get; set; }
    }

    public sealed class FluentSchemaUser
    {
        public int Id { get; init; }
        public string? Email { get; init; }
    }

    [Table("attribute_users", Schema = "attribute_security")]
    public sealed class AttributeSchemaUser
    {
        [Column("attribute_user_id")]
        public int Id { get; init; }

        [Column("email")]
        public string? Email { get; init; }
    }
}
