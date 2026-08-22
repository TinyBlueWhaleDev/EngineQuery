using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Samples.Domain.AttributeMapping;
using TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping;
using TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels;
using TinyBlueWhale.EngineQuery.Samples.Domain.FluentMapping;
using TinyBlueWhale.EngineQuery.Samples.Metadata;

namespace TinyBlueWhale.EngineQuery.Samples.Queries.Scenarios
{
    public static class ProjectionQueries
    {
        public static IReadOnlyList<SalesQueryScenario> CreateForFluent()
        {
            return
            [
                new SalesQueryScenario
                {
                    Name = "Customer scalar functions",
                    MetadataStrategy = MetadataStrategy.Fluent,
                    ResultType = typeof(CustomerEmailFunctionRow),
                    Build = queryBuilder => queryBuilder
                        .From<CustomerFluent>(alias: "c")
                        .Select<CustomerFluent>(customer => new
                        {
                            CustomerId = customer.Id
                        })
                        .SelectScalarFunction<CustomerFluent>(
                            QueryScalarFunction.Upper,
                            customer => customer.Email,
                            "NormalizedEmail")
                        .SelectScalarFunction<CustomerFluent>(
                            QueryScalarFunction.Length,
                            customer => customer.Email,
                            "EmailLength")
                        .SelectScalarFunction<CustomerFluent>(
                            QueryScalarFunction.Coalesce,
                            customer => new object[]
                            {
                                customer.Email,
                                "NO_EMAIL"
                            },
                            "SafeEmail")
                        .SelectScalarFunction<CustomerFluent>(
                            QueryScalarFunction.Concat,
                            customer => new object[]
                            {
                                customer.FullName,
                                " <",
                                customer.Email,
                                ">"
                            },
                            "EmailLabel")
                        .OrderBy<CustomerFluent>(customer => customer.Id)
                        .Build()
                },
                new SalesQueryScenario
                {
                    Name = "Invoice totals with tax",
                    MetadataStrategy = MetadataStrategy.Fluent,
                    ResultType = typeof(InvoiceTotalWithTaxRow),
                    Build = queryBuilder => queryBuilder
                        .From<InvoiceFluent>(alias: "i")
                        .Select<InvoiceFluent>(invoice => new
                        {
                            InvoiceId = invoice.Id,
                            invoice.Total
                        })
                        .SelectComputed<InvoiceFluent>(
                            invoice => invoice.Total * 1.16m,
                            "TotalWithTax")
                        .OrderBy<InvoiceFluent>(invoice => invoice.Id)
                        .Build()
                },
                new SalesQueryScenario
                {
                    Name = "Invoice value classification",
                    MetadataStrategy = MetadataStrategy.Fluent,
                    ResultType = typeof(InvoiceSegmentRow),
                    Build = queryBuilder => queryBuilder
                        .From<InvoiceFluent>(alias: "i")
                        .Select<InvoiceFluent>(invoice => new
                        {
                            InvoiceId = invoice.Id,
                            invoice.Total
                        })
                        .SelectCaseWhen<InvoiceFluent>(
                            condition: invoice => invoice.Total > 1000,
                            whenTrue: "HIGH_VALUE",
                            whenFalse: "STANDARD",
                            alias: "InvoiceSegment")
                        .OrderBy<InvoiceFluent>(invoice => invoice.Id)
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
                    Name = "Customer scalar functions",
                    MetadataStrategy = MetadataStrategy.Attribute,
                    ResultType = typeof(CustomerEmailFunctionRow),
                    Build = queryBuilder => queryBuilder
                        .From<CustomerAttribute>(alias: "c")
                        .Select<CustomerAttribute>(customer => new
                        {
                            CustomerId = customer.Id
                        })
                        .SelectScalarFunction<CustomerAttribute>(
                            QueryScalarFunction.Upper,
                            customer => customer.Email,
                            "NormalizedEmail")
                        .SelectScalarFunction<CustomerAttribute>(
                            QueryScalarFunction.Length,
                            customer => customer.Email,
                            "EmailLength")
                        .SelectScalarFunction<CustomerAttribute>(
                            QueryScalarFunction.Coalesce,
                            customer => new object[]
                            {
                                customer.Email,
                                "NO_EMAIL"
                            },
                            "SafeEmail")
                        .SelectScalarFunction<CustomerAttribute>(
                            QueryScalarFunction.Concat,
                            customer => new object[]
                            {
                                customer.FullName,
                                " <",
                                customer.Email,
                                ">"
                            },
                            "EmailLabel")
                        .OrderBy<CustomerAttribute>(customer => customer.Id)
                        .Build()
                },
                new SalesQueryScenario
                {
                    Name = "Invoice totals with tax",
                    MetadataStrategy = MetadataStrategy.Attribute,
                    ResultType = typeof(InvoiceTotalWithTaxRow),
                    Build = queryBuilder => queryBuilder
                        .From<InvoiceAttribute>(alias: "i")
                        .Select<InvoiceAttribute>(invoice => new
                        {
                            InvoiceId = invoice.Id,
                            invoice.Total
                        })
                        .SelectComputed<InvoiceAttribute>(
                            invoice => invoice.Total * 1.16m,
                            "TotalWithTax")
                        .OrderBy<InvoiceAttribute>(invoice => invoice.Id)
                        .Build()
                },
                new SalesQueryScenario
                {
                    Name = "Invoice value classification",
                    MetadataStrategy = MetadataStrategy.Attribute,
                    ResultType = typeof(InvoiceSegmentRow),
                    Build = queryBuilder => queryBuilder
                        .From<InvoiceAttribute>(alias: "i")
                        .Select<InvoiceAttribute>(invoice => new
                        {
                            InvoiceId = invoice.Id,
                            invoice.Total
                        })
                        .SelectCaseWhen<InvoiceAttribute>(
                            condition: invoice => invoice.Total > 1000,
                            whenTrue: "HIGH_VALUE",
                            whenFalse: "STANDARD",
                            alias: "InvoiceSegment")
                        .OrderBy<InvoiceAttribute>(invoice => invoice.Id)
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
                    Name = "Customer scalar functions",
                    MetadataStrategy = MetadataStrategy.EntityFramework,
                    ResultType = typeof(CustomerEmailFunctionRow),
                    Build = queryBuilder => queryBuilder
                        .From<CustomerEf>(alias: "c")
                        .Select<CustomerEf>(customer => new
                        {
                            CustomerId = customer.Id
                        })
                        .SelectScalarFunction<CustomerEf>(
                            QueryScalarFunction.Upper,
                            customer => customer.Email,
                            "NormalizedEmail")
                        .SelectScalarFunction<CustomerEf>(
                            QueryScalarFunction.Length,
                            customer => customer.Email,
                            "EmailLength")
                        .SelectScalarFunction<CustomerEf>(
                            QueryScalarFunction.Coalesce,
                            customer => new object[]
                            {
                                customer.Email,
                                "NO_EMAIL"
                            },
                            "SafeEmail")
                        .SelectScalarFunction<CustomerEf>(
                            QueryScalarFunction.Concat,
                            customer => new object[]
                            {
                                customer.FullName,
                                " <",
                                customer.Email,
                                ">"
                            },
                            "EmailLabel")
                        .OrderBy<CustomerEf>(customer => customer.Id)
                        .Build()
                },
                new SalesQueryScenario
                {
                    Name = "Invoice totals with tax",
                    MetadataStrategy = MetadataStrategy.EntityFramework,
                    ResultType = typeof(InvoiceTotalWithTaxRow),
                    Build = queryBuilder => queryBuilder
                        .From<InvoiceEf>(alias: "i")
                        .Select<InvoiceEf>(invoice => new
                        {
                            InvoiceId = invoice.Id,
                            invoice.Total
                        })
                        .SelectComputed<InvoiceEf>(
                            invoice => invoice.Total * 1.16m,
                            "TotalWithTax")
                        .OrderBy<InvoiceEf>(invoice => invoice.Id)
                        .Build()
                },
                new SalesQueryScenario
                {
                    Name = "Invoice value classification",
                    MetadataStrategy = MetadataStrategy.EntityFramework,
                    ResultType = typeof(InvoiceSegmentRow),
                    Build = queryBuilder => queryBuilder
                        .From<InvoiceEf>(alias: "i")
                        .Select<InvoiceEf>(invoice => new
                        {
                            InvoiceId = invoice.Id,
                            invoice.Total
                        })
                        .SelectCaseWhen<InvoiceEf>(
                            condition: invoice => invoice.Total > 1000,
                            whenTrue: "HIGH_VALUE",
                            whenFalse: "STANDARD",
                            alias: "InvoiceSegment")
                        .OrderBy<InvoiceEf>(invoice => invoice.Id)
                        .Build()
                }
            ];
        }
    }
}
