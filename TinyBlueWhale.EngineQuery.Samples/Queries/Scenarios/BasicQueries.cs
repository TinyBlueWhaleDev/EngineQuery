using TinyBlueWhale.EngineQuery.Samples.Domain.AttributeMapping;
using TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping;
using TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels;
using TinyBlueWhale.EngineQuery.Samples.Domain.FluentMapping;
using TinyBlueWhale.EngineQuery.Samples.Metadata;

namespace TinyBlueWhale.EngineQuery.Samples.Queries.Scenarios
{
    //public static class BasicQueries
    //{
    //    public static IReadOnlyList<SalesQueryScenario> CreateForFluent()
    //    {
    //        return
    //        [
    //            new SalesQueryScenario
    //            {
    //                Name = "Active customers",
    //                MetadataStrategy = MetadataStrategy.Fluent,
    //                ResultType = typeof(ActiveCustomerRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerFluent>(alias: "c")
    //                    .Select<CustomerFluent>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.Email,
    //                        customer.FullName
    //                    })
    //                    .Where<CustomerFluent>(customer => customer.IsActive)
    //                    .OrderBy<CustomerFluent>(customer => customer.FullName)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Distinct customer emails",
    //                MetadataStrategy = MetadataStrategy.Fluent,
    //                ResultType = typeof(CustomerEmailRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerFluent>(alias: "c")
    //                    .Distinct()
    //                    .Select<CustomerFluent>(customer => new
    //                    {
    //                        customer.Email
    //                    })
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "High value invoices",
    //                MetadataStrategy = MetadataStrategy.Fluent,
    //                ResultType = typeof(InvoiceRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<InvoiceFluent>(alias: "i")
    //                    .Select<InvoiceFluent>(invoice => new
    //                    {
    //                        InvoiceId = invoice.Id,
    //                        invoice.InvoiceNumber,
    //                        invoice.Total,
    //                        invoice.CreatedAt
    //                    })
    //                    .Where<InvoiceFluent>(invoice => invoice.Total > 500)
    //                    .OrderByDescending<InvoiceFluent>(invoice => invoice.Total)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Paged high value invoices",
    //                MetadataStrategy = MetadataStrategy.Fluent,
    //                ResultType = typeof(InvoiceRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<InvoiceFluent>(alias: "i")
    //                    .Select<InvoiceFluent>(invoice => new
    //                    {
    //                        InvoiceId = invoice.Id,
    //                        invoice.InvoiceNumber,
    //                        invoice.Total,
    //                        invoice.CreatedAt
    //                    })
    //                    .Where<InvoiceFluent>(invoice => invoice.Total > 100)
    //                    .OrderByDescending<InvoiceFluent>(invoice => invoice.CreatedAt)
    //                    .Skip(0)
    //                    .Take(5)
    //                    .Build()
    //            }
    //        ];
    //    }

    //    public static IReadOnlyList<SalesQueryScenario> CreateForAttribute()
    //    {
    //        return
    //        [
    //            new SalesQueryScenario
    //            {
    //                Name = "Active customers",
    //                MetadataStrategy = MetadataStrategy.Attribute,
    //                ResultType = typeof(ActiveCustomerRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerAttribute>(alias: "c")
    //                    .Select<CustomerAttribute>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.Email,
    //                        customer.FullName
    //                    })
    //                    .Where<CustomerAttribute>(customer => customer.IsActive)
    //                    .OrderBy<CustomerAttribute>(customer => customer.FullName)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Distinct customer emails",
    //                MetadataStrategy = MetadataStrategy.Attribute,
    //                ResultType = typeof(CustomerEmailRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerAttribute>(alias: "c")
    //                    .Distinct()
    //                    .Select<CustomerAttribute>(customer => new
    //                    {
    //                        customer.Email
    //                    })
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "High value invoices",
    //                MetadataStrategy = MetadataStrategy.Attribute,
    //                ResultType = typeof(InvoiceRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<InvoiceAttribute>(alias: "i")
    //                    .Select<InvoiceAttribute>(invoice => new
    //                    {
    //                        InvoiceId = invoice.Id,
    //                        invoice.InvoiceNumber,
    //                        invoice.Total,
    //                        invoice.CreatedAt
    //                    })
    //                    .Where<InvoiceAttribute>(invoice => invoice.Total > 500)
    //                    .OrderByDescending<InvoiceAttribute>(invoice => invoice.Total)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Paged high value invoices",
    //                MetadataStrategy = MetadataStrategy.Attribute,
    //                ResultType = typeof(InvoiceRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<InvoiceAttribute>(alias: "i")
    //                    .Select<InvoiceAttribute>(invoice => new
    //                    {
    //                        InvoiceId = invoice.Id,
    //                        invoice.InvoiceNumber,
    //                        invoice.Total,
    //                        invoice.CreatedAt
    //                    })
    //                    .Where<InvoiceAttribute>(invoice => invoice.Total > 100)
    //                    .OrderByDescending<InvoiceAttribute>(invoice => invoice.CreatedAt)
    //                    .Skip(0)
    //                    .Take(5)
    //                    .Build()
    //            }
    //        ];
    //    }

    //    public static IReadOnlyList<SalesQueryScenario> CreateForEntityFramework()
    //    {
    //        return
    //        [
    //            new SalesQueryScenario
    //            {
    //                Name = "Active customers",
    //                MetadataStrategy = MetadataStrategy.EntityFramework,
    //                ResultType = typeof(ActiveCustomerRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerEf>(alias: "c")
    //                    .Select<CustomerEf>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.Email,
    //                        FullName = customer.FullName
    //                    })
    //                    .Where<CustomerEf>(customer => customer.IsActive)
    //                    .OrderBy<CustomerEf>(customer => customer.FullName)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Distinct customer emails",
    //                MetadataStrategy = MetadataStrategy.EntityFramework,
    //                ResultType = typeof(CustomerEmailRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerEf>(alias: "c")
    //                    .Distinct()
    //                    .Select<CustomerEf>(customer => new
    //                    {
    //                        customer.Email
    //                    })
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "High value invoices",
    //                MetadataStrategy = MetadataStrategy.EntityFramework,
    //                ResultType = typeof(InvoiceRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<InvoiceEf>(alias: "i")
    //                    .Select<InvoiceEf>(invoice => new
    //                    {
    //                        InvoiceId = invoice.Id,
    //                        invoice.InvoiceNumber,
    //                        invoice.Total,
    //                        invoice.CreatedAt
    //                    })
    //                    .Where<InvoiceEf>(invoice => invoice.Total > 500)
    //                    .OrderByDescending<InvoiceEf>(invoice => invoice.Total)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Paged high value invoices",
    //                MetadataStrategy = MetadataStrategy.EntityFramework,
    //                ResultType = typeof(InvoiceRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<InvoiceEf>(alias: "i")
    //                    .Select<InvoiceEf>(invoice => new
    //                    {
    //                        InvoiceId = invoice.Id,
    //                        invoice.InvoiceNumber,
    //                        invoice.Total,
    //                        invoice.CreatedAt
    //                    })
    //                    .Where<InvoiceEf>(invoice => invoice.Total > 100)
    //                    .OrderByDescending<InvoiceEf>(invoice => invoice.CreatedAt)
    //                    .Skip(0)
    //                    .Take(5)
    //                    .Build()
    //            }
    //        ];
    //    }
    //}
}
