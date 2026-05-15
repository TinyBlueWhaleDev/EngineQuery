using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Metadata.Fluent;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Tests.Models;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{
    /// <summary>
    /// Creates metadata resolvers used by query compiler snapshot tests.
    /// </summary>
    internal static class TestMetadataFactory
    {
        /// <summary>
        /// Creates the default metadata resolver used by SQL generation tests.
        /// </summary>
        public static FluentEntityMetadataResolver CreateMetadataResolver()
        {
            var registry = new EntityMetadataRegistry();

            registry.Entity<User>()
                .ToTable("Users")
                .Property(x => x.Id).HasColumnName("Id")
                .Property(x => x.Email).HasColumnName("Email")
                .Property(x => x.IsActive).HasColumnName("IsActive")
                .Property(x => x.IsDeleted).HasColumnName("IsDeleted")
                .Property(x => x.Age).HasColumnName("Age")
                .Property(x => x.CreatedAt).HasColumnName("CreatedAt");

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

            registry.Entity<OrderSummary>()
                .ToTable("order_summary")
                .Property(x => x.UserId).HasColumnName("UserId")
                .Property(x => x.TotalAmount).HasColumnName("TotalAmount")
                .Property(x => x.OrderCount).HasColumnName("OrderCount");

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
