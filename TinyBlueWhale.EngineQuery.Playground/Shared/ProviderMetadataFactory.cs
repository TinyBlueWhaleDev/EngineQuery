using TinyBlueWhale.EngineQuery.Metadata.Fluent;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Playground.Models;

namespace TinyBlueWhale.EngineQuery.Playground.Shared
{
    /// <summary>
    /// Creates shared playground metadata used by provider comparison validators.
    /// </summary>
    internal static class ProviderMetadataFactory
    {
        /// <summary>
        /// Creates metadata mappings for join validation models.
        /// </summary>
        public static FluentEntityMetadataResolver CreateJoinMetadataResolver()
        {
            var registry = new EntityMetadataRegistry();

            registry.Entity<JoinUser>()
                .ToTable("users")
                .Property(x => x.Id).HasColumnName("user_id")
                .Property(x => x.Email).HasColumnName("email")
                .Property(x => x.IsActive).HasColumnName("is_active");

            registry.Entity<JoinOrder>()
                .ToTable("orders")
                .Property(x => x.Id).HasColumnName("order_id")
                .Property(x => x.UserId).HasColumnName("user_id")
                .Property(x => x.Total).HasColumnName("total");

            registry.Entity<JoinOrderItem>()
                .ToTable("order_items")
                .Property(x => x.Id).HasColumnName("order_item_id")
                .Property(x => x.OrderId).HasColumnName("order_id")
                .Property(x => x.Quantity).HasColumnName("quantity");

            registry.Entity<ActiveUser>()
                .ToTable("users")
                .Property(x => x.Id).HasColumnName("user_id")
                .Property(x => x.Email).HasColumnName("email");

            registry.Entity<ArchivedUser>()
                .ToTable("archived_users")
                .Property(x => x.Id).HasColumnName("archived_user_id")
                .Property(x => x.Email).HasColumnName("email");

            registry.Entity<Category>()
                .ToTable("categories")
                .Property(x => x.Id).HasColumnName("category_id")
                .Property(x => x.ParentId).HasColumnName("parent_category_id")
                .Property(x => x.Name).HasColumnName("name");

            registry.Entity<CategoryTree>()
                .ToTable("category_tree")
                .Property(x => x.Id).HasColumnName("Id")
                .Property(x => x.ParentId).HasColumnName("ParentId")
                .Property(x => x.Name).HasColumnName("Name");

            return new FluentEntityMetadataResolver(registry);
        }
    }
}
