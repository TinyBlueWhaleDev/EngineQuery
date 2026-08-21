using Microsoft.EntityFrameworkCore;
using TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping;
using TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels;

namespace TinyBlueWhale.EngineQuery.Samples.EntityFramework;

public sealed class SampleDbContext(DbContextOptions<SampleDbContext> options) : DbContext(options)
{
    public DbSet<CustomerEf> Customers => Set<CustomerEf>();

    public DbSet<InvoiceEf> Invoices => Set<InvoiceEf>();

    public DbSet<ProductEf> Products => Set<ProductEf>();

    public DbSet<InvoiceLineEf> InvoiceLines => Set<InvoiceLineEf>();

    public DbSet<CategoryEf> Categories => Set<CategoryEf>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerEf>(entity =>
        {
            entity.ToTable("customers");
            entity.HasKey(customer => customer.Id);
            entity.Property(customer => customer.Id).HasColumnName("customer_id");
            entity.Property(customer => customer.Email).HasColumnName("email");
            entity.Property(customer => customer.FullName).HasColumnName("full_name");
            entity.Property(customer => customer.IsActive).HasColumnName("is_active");
            entity.Property(customer => customer.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<InvoiceEf>(entity =>
        {
            entity.ToTable("invoices");
            entity.HasKey(invoice => invoice.Id);
            entity.Property(invoice => invoice.Id).HasColumnName("invoice_id");
            entity.Property(invoice => invoice.CustomerId).HasColumnName("customer_id");
            entity.Property(invoice => invoice.InvoiceNumber).HasColumnName("invoice_number");
            entity.Property(invoice => invoice.Total).HasColumnName("total");
            entity.Property(invoice => invoice.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<ProductEf>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Id).HasColumnName("product_id");
            entity.Property(product => product.Name).HasColumnName("name");
            entity.Property(product => product.UnitPrice).HasColumnName("unit_price");
            entity.Property(product => product.IsActive).HasColumnName("is_active");
        });

        modelBuilder.Entity<InvoiceLineEf>(entity =>
        {
            entity.ToTable("invoice_lines");
            entity.HasKey(line => line.Id);
            entity.Property(line => line.Id).HasColumnName("invoice_line_id");
            entity.Property(line => line.InvoiceId).HasColumnName("invoice_id");
            entity.Property(line => line.ProductId).HasColumnName("product_id");
            entity.Property(line => line.Quantity).HasColumnName("quantity");
            entity.Property(line => line.LineTotal).HasColumnName("line_total");
        });

        modelBuilder.Entity<CategoryEf>(entity =>
        {
            entity.ToTable("categories");
            entity.HasKey(category => category.Id);
            entity.Property(category => category.Id).HasColumnName("category_id");
            entity.Property(category => category.ParentId).HasColumnName("parent_category_id");
            entity.Property(category => category.Name).HasColumnName("name");
        });

        modelBuilder.Entity<CategoryTreeRow>(entity =>
        {
            entity.HasNoKey();
            entity.ToTable("category_tree");
            entity.Property(category => category.Id).HasColumnName("Id");
            entity.Property(category => category.ParentId).HasColumnName("ParentId");
            entity.Property(category => category.Name).HasColumnName("Name");
        });

        modelBuilder.Entity<ActiveCustomerRow>().HasNoKey();
        modelBuilder.Entity<CustomerEmailRow>().HasNoKey();
        modelBuilder.Entity<InvoiceRow>().HasNoKey();
        modelBuilder.Entity<CustomerInvoiceSummaryRow>().HasNoKey();
        modelBuilder.Entity<ProductRevenueSummaryRow>().HasNoKey();
        modelBuilder.Entity<AverageInvoiceAmountRow>().HasNoKey();
        modelBuilder.Entity<MaxInvoicePerCustomerRow>().HasNoKey();
        modelBuilder.Entity<CustomerLookupRow>().HasNoKey();
        modelBuilder.Entity<InvoiceTotalWithTaxRow>().HasNoKey();
        modelBuilder.Entity<InvoiceSegmentRow>().HasNoKey();
        modelBuilder.Entity<CustomerEmailFunctionRow>().HasNoKey();
        modelBuilder.Entity<CustomerInvoiceTotalRow>().HasNoKey();
        modelBuilder.Entity<LatestInvoicePerCustomerRow>().HasNoKey();
        modelBuilder.Entity<InvoiceRankingRow>().HasNoKey();
        modelBuilder.Entity<InvoiceRankDenseRankRow>().HasNoKey();
        modelBuilder.Entity<InvoiceLagLeadRow>().HasNoKey();
        modelBuilder.Entity<InvoiceFirstLastValueRow>().HasNoKey();
        modelBuilder.Entity<InvoiceQuartileRow>().HasNoKey();
        modelBuilder.Entity<CustomerOptionalInvoiceRow>().HasNoKey();
        modelBuilder.Entity<MinInvoicePerCustomerRow>().HasNoKey();
    }
}
