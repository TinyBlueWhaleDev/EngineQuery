using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Metadata.Fluent;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels;
using TinyBlueWhale.EngineQuery.Samples.Domain.FluentMapping;

namespace TinyBlueWhale.EngineQuery.Samples.Metadata
{
    public static class BuildFluentMetadata
    {
        public static IEntityMetadataResolver Create()
        {
            var registry = new EntityMetadataRegistry();

            registry.Entity<CustomerFluent>()
                .ToTable("customers")
                .Property(customer => customer.Id).HasColumnName("customer_id")
                .Property(customer => customer.Email).HasColumnName("email")
                .Property(customer => customer.FullName).HasColumnName("full_name")
                .Property(customer => customer.IsActive).HasColumnName("is_active")
                .Property(customer => customer.CreatedAt).HasColumnName("created_at");

            registry.Entity<InvoiceFluent>()
                .ToTable("invoices")
                .Property(invoice => invoice.Id).HasColumnName("invoice_id")
                .Property(invoice => invoice.CustomerId).HasColumnName("customer_id")
                .Property(invoice => invoice.InvoiceNumber).HasColumnName("invoice_number")
                .Property(invoice => invoice.Total).HasColumnName("total")
                .Property(invoice => invoice.CreatedAt).HasColumnName("created_at");

            registry.Entity<ProductFluent>()
                .ToTable("products")
                .Property(product => product.Id).HasColumnName("product_id")
                .Property(product => product.Name).HasColumnName("name")
                .Property(product => product.UnitPrice).HasColumnName("unit_price")
                .Property(product => product.IsActive).HasColumnName("is_active");

            registry.Entity<InvoiceLineFluent>()
                .ToTable("invoice_lines")
                .Property(line => line.Id).HasColumnName("invoice_line_id")
                .Property(line => line.InvoiceId).HasColumnName("invoice_id")
                .Property(line => line.ProductId).HasColumnName("product_id")
                .Property(line => line.Quantity).HasColumnName("quantity")
                .Property(line => line.LineTotal).HasColumnName("line_total");

            registry.Entity<CategoryFluent>()
                .ToTable("categories")
                .Property(category => category.Id).HasColumnName("category_id")
                .Property(category => category.ParentId).HasColumnName("parent_category_id")
                .Property(category => category.Name).HasColumnName("name");

            registry.Entity<CategoryTreeRow>()
                .ToTable("category_tree")
                .Property(category => category.Id).HasColumnName("Id")
                .Property(category => category.ParentId).HasColumnName("ParentId")
                .Property(category => category.Name).HasColumnName("Name");

            return new FluentEntityMetadataResolver(registry);
        }
    }
}
