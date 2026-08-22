using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Samples.Domain.AttributeMapping;
using TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping;
using TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels;
using TinyBlueWhale.EngineQuery.Samples.Domain.FluentMapping;
using TinyBlueWhale.EngineQuery.Samples.Metadata;

namespace TinyBlueWhale.EngineQuery.Samples.Queries.Scenarios
{
    public static class PredicateQueries
    {
        public static IReadOnlyList<SalesQueryScenario> CreateForFluent()
        {
            return
            [
                new SalesQueryScenario
                {
                    Name = "Invoices included by identifier collection",
                    MetadataStrategy = MetadataStrategy.Fluent,
                    ResultType = typeof(InvoiceRow),
                    Build = queryBuilder => queryBuilder
                        .From<InvoiceFluent>(alias: "i")
                        .Select<InvoiceFluent>(invoice => new
                        {
                            InvoiceId = invoice.Id,
                            invoice.InvoiceNumber,
                            invoice.Total,
                            invoice.CreatedAt
                        })
                        .WhereIn(invoice => invoice.Id, new[] { 1, 3 })
                        .OrderBy<InvoiceFluent>(invoice => invoice.Id)
                        .Build()
                },
                new SalesQueryScenario
                {
                    Name = "Invoices excluded by identifier collection",
                    MetadataStrategy = MetadataStrategy.Fluent,
                    ResultType = typeof(InvoiceRow),
                    Build = queryBuilder => queryBuilder
                        .From<InvoiceFluent>(alias: "i")
                        .Select<InvoiceFluent>(invoice => new
                        {
                            InvoiceId = invoice.Id,
                            invoice.InvoiceNumber,
                            invoice.Total,
                            invoice.CreatedAt
                        })
                        .WhereNotIn(invoice => invoice.Id, new[] { 2, 4 })
                        .OrderBy<InvoiceFluent>(invoice => invoice.Id)
                        .Build()
                },
                new SalesQueryScenario
                {
                    Name = "Optional customer name alternatives",
                    MetadataStrategy = MetadataStrategy.Fluent,
                    ResultType = typeof(ActiveCustomerRow),
                    Build = queryBuilder =>
                    {
                        string? primaryNamePrefix = "Admin";
                        string? alternativeNamePrefix = "Reader";

                        return queryBuilder
                            .From<CustomerFluent>(alias: "c")
                            .Select<CustomerFluent>(customer => new
                            {
                                CustomerId = customer.Id,
                                customer.Email,
                                customer.FullName
                            })
                            .WhereIf(
                                !string.IsNullOrWhiteSpace(primaryNamePrefix),
                                customer => customer.FullName.StartsWith(primaryNamePrefix))
                            .WhereIf(
                                !string.IsNullOrWhiteSpace(alternativeNamePrefix),
                                customer => customer.FullName.StartsWith(alternativeNamePrefix),
                                QueryLogicalOperator.Or)
                            .OrderBy<CustomerFluent>(customer => customer.FullName)
                            .ThenBy<CustomerFluent>(customer => customer.Email)
                            .Build();
                    }
                },
                new SalesQueryScenario
                {
                    Name = "Customer email scalar filter",
                    MetadataStrategy = MetadataStrategy.Fluent,
                    ResultType = typeof(CustomerLookupRow),
                    Build = queryBuilder => queryBuilder
                        .From<CustomerFluent>(alias: "c")
                        .Select<CustomerFluent>(customer => new
                        {
                            CustomerId = customer.Id,
                            customer.Email
                        })
                        .WhereScalarFunction<CustomerFluent>(
                            QueryScalarFunction.Lower,
                            customer => customer.Email,
                            QueryComparisonOperator.Equal,
                            "admin@test.com")
                        .OrderBy<CustomerFluent>(customer => customer.Id)
                        .Build()
                }
            ];
        }

        public static IReadOnlyList<SalesQueryScenario> CreateForAttribute()
        {
            return
            [
                new SalesQueryScenario
                {
                    Name = "Invoices included by identifier collection",
                    MetadataStrategy = MetadataStrategy.Attribute,
                    ResultType = typeof(InvoiceRow),
                    Build = queryBuilder => queryBuilder
                        .From<InvoiceAttribute>(alias: "i")
                        .Select<InvoiceAttribute>(invoice => new
                        {
                            InvoiceId = invoice.Id,
                            invoice.InvoiceNumber,
                            invoice.Total,
                            invoice.CreatedAt
                        })
                        .WhereIn(invoice => invoice.Id, new[] { 1, 3 })
                        .OrderBy<InvoiceAttribute>(invoice => invoice.Id)
                        .Build()
                },
                new SalesQueryScenario
                {
                    Name = "Invoices excluded by identifier collection",
                    MetadataStrategy = MetadataStrategy.Attribute,
                    ResultType = typeof(InvoiceRow),
                    Build = queryBuilder => queryBuilder
                        .From<InvoiceAttribute>(alias: "i")
                        .Select<InvoiceAttribute>(invoice => new
                        {
                            InvoiceId = invoice.Id,
                            invoice.InvoiceNumber,
                            invoice.Total,
                            invoice.CreatedAt
                        })
                        .WhereNotIn(invoice => invoice.Id, new[] { 2, 4 })
                        .OrderBy<InvoiceAttribute>(invoice => invoice.Id)
                        .Build()
                },
                new SalesQueryScenario
                {
                    Name = "Optional customer name alternatives",
                    MetadataStrategy = MetadataStrategy.Attribute,
                    ResultType = typeof(ActiveCustomerRow),
                    Build = queryBuilder =>
                    {
                        string? primaryNamePrefix = "Admin";
                        string? alternativeNamePrefix = "Reader";

                        return queryBuilder
                            .From<CustomerAttribute>(alias: "c")
                            .Select<CustomerAttribute>(customer => new
                            {
                                CustomerId = customer.Id,
                                customer.Email,
                                customer.FullName
                            })
                            .WhereIf(
                                !string.IsNullOrWhiteSpace(primaryNamePrefix),
                                customer => customer.FullName.StartsWith(primaryNamePrefix))
                            .WhereIf(
                                !string.IsNullOrWhiteSpace(alternativeNamePrefix),
                                customer => customer.FullName.StartsWith(alternativeNamePrefix),
                                QueryLogicalOperator.Or)
                            .OrderBy<CustomerAttribute>(customer => customer.FullName)
                            .ThenBy<CustomerAttribute>(customer => customer.Email)
                            .Build();
                    }
                },
                new SalesQueryScenario
                {
                    Name = "Customer email scalar filter",
                    MetadataStrategy = MetadataStrategy.Attribute,
                    ResultType = typeof(CustomerLookupRow),
                    Build = queryBuilder => queryBuilder
                        .From<CustomerAttribute>(alias: "c")
                        .Select<CustomerAttribute>(customer => new
                        {
                            CustomerId = customer.Id,
                            customer.Email
                        })
                        .WhereScalarFunction<CustomerAttribute>(
                            QueryScalarFunction.Lower,
                            customer => customer.Email,
                            QueryComparisonOperator.Equal,
                            "admin@test.com")
                        .OrderBy<CustomerAttribute>(customer => customer.Id)
                        .Build()
                }
            ];
        }

        public static IReadOnlyList<SalesQueryScenario> CreateForEntityFramework()
        {
            return
            [
                new SalesQueryScenario
                {
                    Name = "Invoices included by identifier collection",
                    MetadataStrategy = MetadataStrategy.EntityFramework,
                    ResultType = typeof(InvoiceRow),
                    Build = queryBuilder => queryBuilder
                        .From<InvoiceEf>(alias: "i")
                        .Select<InvoiceEf>(invoice => new
                        {
                            InvoiceId = invoice.Id,
                            invoice.InvoiceNumber,
                            invoice.Total,
                            invoice.CreatedAt
                        })
                        .WhereIn(invoice => invoice.Id, new[] { 1, 3 })
                        .OrderBy<InvoiceEf>(invoice => invoice.Id)
                        .Build()
                },
                new SalesQueryScenario
                {
                    Name = "Invoices excluded by identifier collection",
                    MetadataStrategy = MetadataStrategy.EntityFramework,
                    ResultType = typeof(InvoiceRow),
                    Build = queryBuilder => queryBuilder
                        .From<InvoiceEf>(alias: "i")
                        .Select<InvoiceEf>(invoice => new
                        {
                            InvoiceId = invoice.Id,
                            invoice.InvoiceNumber,
                            invoice.Total,
                            invoice.CreatedAt
                        })
                        .WhereNotIn(invoice => invoice.Id, new[] { 2, 4 })
                        .OrderBy<InvoiceEf>(invoice => invoice.Id)
                        .Build()
                },
                new SalesQueryScenario
                {
                    Name = "Optional customer name alternatives",
                    MetadataStrategy = MetadataStrategy.EntityFramework,
                    ResultType = typeof(ActiveCustomerRow),
                    Build = queryBuilder =>
                    {
                        string? primaryNamePrefix = "Admin";
                        string? alternativeNamePrefix = "Reader";

                        return queryBuilder
                            .From<CustomerEf>(alias: "c")
                            .Select<CustomerEf>(customer => new
                            {
                                CustomerId = customer.Id,
                                customer.Email,
                                customer.FullName
                            })
                            .WhereIf(
                                !string.IsNullOrWhiteSpace(primaryNamePrefix),
                                customer => customer.FullName.StartsWith(primaryNamePrefix))
                            .WhereIf(
                                !string.IsNullOrWhiteSpace(alternativeNamePrefix),
                                customer => customer.FullName.StartsWith(alternativeNamePrefix),
                                QueryLogicalOperator.Or)
                            .OrderBy<CustomerEf>(customer => customer.FullName)
                            .ThenBy<CustomerEf>(customer => customer.Email)
                            .Build();
                    }
                },
                new SalesQueryScenario
                {
                    Name = "Customer email scalar filter",
                    MetadataStrategy = MetadataStrategy.EntityFramework,
                    ResultType = typeof(CustomerLookupRow),
                    Build = queryBuilder => queryBuilder
                        .From<CustomerEf>(alias: "c")
                        .Select<CustomerEf>(customer => new
                        {
                            CustomerId = customer.Id,
                            customer.Email
                        })
                        .WhereScalarFunction<CustomerEf>(
                            QueryScalarFunction.Lower,
                            customer => customer.Email,
                            QueryComparisonOperator.Equal,
                            "admin@test.com")
                        .OrderBy<CustomerEf>(customer => customer.Id)
                        .Build()
                }
            ];
        }
    }
}
