using TinyBlueWhale.EngineQuery.Labs.Domain.Entities;
using TinyBlueWhale.EngineQuery.Metadata.Fluent;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;

namespace TinyBlueWhale.EngineQuery.Labs.Infrastructure.Persistence.FluentMappings;

public static class EngineQueryMappings
{
    public static FluentEntityMetadataResolver CreateResolver()
    {
        var registry = new EntityMetadataRegistry();
        registry.Entity<Customer>()
            .ToTable("Customers")
            .Property(x => x.Id).HasColumnName("Id")
            .Property(x => x.FirstName).HasColumnName("FirstName")
            .Property(x => x.LastName).HasColumnName("LastName")
            .Property(x => x.Email).HasColumnName("Email")
            .Property(x => x.Country).HasColumnName("Country")
            .Property(x => x.IsActive).HasColumnName("IsActive")
            .Property(x => x.CreatedAtUtc).HasColumnName("CreatedAtUtc");

        registry.Entity<Category>()
            .ToTable("Categories")
            .Property(x => x.Id).HasColumnName("Id")
            .Property(x => x.Name).HasColumnName("Name")
            .Property(x => x.Description).HasColumnName("Description")
            .Property(x => x.IsActive).HasColumnName("IsActive")
            .Property(x => x.CreatedAtUtc).HasColumnName("CreatedAtUtc");

        registry.Entity<Product>()
            .ToTable("Products")
            .Property(x => x.Id).HasColumnName("Id")
            .Property(x => x.CategoryId).HasColumnName("CategoryId")
            .Property(x => x.Name).HasColumnName("Name")
            .Property(x => x.Sku).HasColumnName("Sku")
            .Property(x => x.UnitPrice).HasColumnName("UnitPrice")
            .Property(x => x.IsActive).HasColumnName("IsActive")
            .Property(x => x.CreatedAtUtc).HasColumnName("CreatedAtUtc");

        registry.Entity<Order>()
            .ToTable("Orders")
            .Property(x => x.Id).HasColumnName("Id")
            .Property(x => x.CustomerId).HasColumnName("CustomerId")
            .Property(x => x.OrderNumber).HasColumnName("OrderNumber")
            .Property(x => x.Status).HasColumnName("Status")
            .Property(x => x.OrderDateUtc).HasColumnName("OrderDateUtc")
            .Property(x => x.TotalAmount).HasColumnName("TotalAmount")
            .Property(x => x.CreatedAtUtc).HasColumnName("CreatedAtUtc");

        registry.Entity<OrderItem>()
            .ToTable("OrderItems")
            .Property(x => x.Id).HasColumnName("Id")
            .Property(x => x.OrderId).HasColumnName("OrderId")
            .Property(x => x.ProductId).HasColumnName("ProductId")
            .Property(x => x.Quantity).HasColumnName("Quantity")
            .Property(x => x.UnitPrice).HasColumnName("UnitPrice")
            .Property(x => x.LineTotal).HasColumnName("LineTotal")
            .Property(x => x.CreatedAtUtc).HasColumnName("CreatedAtUtc");

        registry.Entity<OrderSearchText>()
            .ToTable("Lab001OrderSearchText")
            .Property(x => x.OrderId).HasColumnName("OrderId")
            .Property(x => x.SearchText).HasColumnName("SearchText");
        return new FluentEntityMetadataResolver(registry);
    }
}
