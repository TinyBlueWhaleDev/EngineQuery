namespace TinyBlueWhale.EngineQuery.Samples.Queries.Scenarios
{
    //public static class JoinQueries
    //{
    //    public static IReadOnlyList<SalesQueryScenario> CreateForFluent()
    //    {
    //        return
    //        [
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers with optional invoices",
    //                MetadataStrategy = MetadataStrategy.Fluent,
    //                ResultType = typeof(CustomerOptionalInvoiceRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerFluent>(alias: "c")
    //                    .LeftJoin<CustomerFluent, InvoiceFluent>(
    //                        alias: "i",
    //                        on: (customer, invoice) => customer.Id == invoice.CustomerId)
    //                    .Select<CustomerFluent>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.FullName
    //                    })
    //                    .Select<InvoiceFluent>(invoice => new
    //                    {
    //                        InvoiceId = invoice.Id,
    //                        invoice.InvoiceNumber,
    //                        invoice.Total
    //                    })
    //                    .OrderBy<CustomerFluent>(customer => customer.Id)
    //                    .ThenBy<InvoiceFluent>(invoice => invoice.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customer invoices created after registration",
    //                MetadataStrategy = MetadataStrategy.Fluent,
    //                ResultType = typeof(CustomerOptionalInvoiceRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerFluent>(alias: "c")
    //                    .InnerJoin<CustomerFluent, InvoiceFluent>(
    //                        alias: "i",
    //                        on: (customer, invoice) =>
    //                            customer.Id == invoice.CustomerId &&
    //                            customer.CreatedAt <= invoice.CreatedAt)
    //                    .Select<CustomerFluent>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.FullName
    //                    })
    //                    .Select<InvoiceFluent>(invoice => new
    //                    {
    //                        InvoiceId = invoice.Id,
    //                        invoice.InvoiceNumber,
    //                        invoice.Total
    //                    })
    //                    .OrderBy<CustomerFluent>(customer => customer.Id)
    //                    .ThenBy<InvoiceFluent>(invoice => invoice.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers with latest invoice",
    //                MetadataStrategy = MetadataStrategy.Fluent,
    //                ResultType = typeof(LatestInvoicePerCustomerRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerFluent>(alias: "c")
    //                    .Select<CustomerFluent>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.FullName
    //                    })
    //                    .CrossApply<CustomerFluent, InvoiceFluent>(
    //                        alias: "latest_invoice",
    //                        apply => apply
    //                            .Select<InvoiceFluent>(invoice => new
    //                            {
    //                                InvoiceId = invoice.Id,
    //                                invoice.CustomerId,
    //                                invoice.Total
    //                            })
    //                            .WhereComputed<InvoiceFluent, CustomerFluent>(
    //                                (invoice, customer) => invoice.CustomerId == customer.Id)
    //                            .OrderByDescending<InvoiceFluent>(invoice => invoice.CreatedAt)
    //                            .Take(1))
    //                    .OrderBy<CustomerFluent>(customer => customer.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers with optional latest invoice",
    //                MetadataStrategy = MetadataStrategy.Fluent,
    //                ResultType = typeof(LatestInvoicePerCustomerRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerFluent>(alias: "c")
    //                    .Select<CustomerFluent>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.FullName
    //                    })
    //                    .OuterApply<CustomerFluent, InvoiceFluent>(
    //                        alias: "latest_invoice",
    //                        apply => apply
    //                            .Select<InvoiceFluent>(invoice => new
    //                            {
    //                                InvoiceId = invoice.Id,
    //                                invoice.CustomerId,
    //                                invoice.Total
    //                            })
    //                            .WhereComputed<InvoiceFluent, CustomerFluent>(
    //                                (invoice, customer) => invoice.CustomerId == customer.Id)
    //                            .OrderByDescending<InvoiceFluent>(invoice => invoice.CreatedAt)
    //                            .Take(1))
    //                    .OrderBy<CustomerFluent>(customer => customer.Id)
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
    //                Name = "Customers with optional invoices",
    //                MetadataStrategy = MetadataStrategy.Attribute,
    //                ResultType = typeof(CustomerOptionalInvoiceRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerAttribute>(alias: "c")
    //                    .LeftJoin<CustomerAttribute, InvoiceAttribute>(
    //                        alias: "i",
    //                        on: (customer, invoice) => customer.Id == invoice.CustomerId)
    //                    .Select<CustomerAttribute>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.FullName
    //                    })
    //                    .Select<InvoiceAttribute>(invoice => new
    //                    {
    //                        InvoiceId = invoice.Id,
    //                        invoice.InvoiceNumber,
    //                        invoice.Total
    //                    })
    //                    .OrderBy<CustomerAttribute>(customer => customer.Id)
    //                    .ThenBy<InvoiceAttribute>(invoice => invoice.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customer invoices created after registration",
    //                MetadataStrategy = MetadataStrategy.Attribute,
    //                ResultType = typeof(CustomerOptionalInvoiceRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerAttribute>(alias: "c")
    //                    .InnerJoin<CustomerAttribute, InvoiceAttribute>(
    //                        alias: "i",
    //                        on: (customer, invoice) =>
    //                            customer.Id == invoice.CustomerId &&
    //                            customer.CreatedAt <= invoice.CreatedAt)
    //                    .Select<CustomerAttribute>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.FullName
    //                    })
    //                    .Select<InvoiceAttribute>(invoice => new
    //                    {
    //                        InvoiceId = invoice.Id,
    //                        invoice.InvoiceNumber,
    //                        invoice.Total
    //                    })
    //                    .OrderBy<CustomerAttribute>(customer => customer.Id)
    //                    .ThenBy<InvoiceAttribute>(invoice => invoice.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers with latest invoice",
    //                MetadataStrategy = MetadataStrategy.Attribute,
    //                ResultType = typeof(LatestInvoicePerCustomerRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerAttribute>(alias: "c")
    //                    .Select<CustomerAttribute>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.FullName
    //                    })
    //                    .CrossApply<CustomerAttribute, InvoiceAttribute>(
    //                        alias: "latest_invoice",
    //                        apply => apply
    //                            .Select<InvoiceAttribute>(invoice => new
    //                            {
    //                                InvoiceId = invoice.Id,
    //                                invoice.CustomerId,
    //                                invoice.Total
    //                            })
    //                            .WhereComputed<InvoiceAttribute, CustomerAttribute>(
    //                                (invoice, customer) => invoice.CustomerId == customer.Id)
    //                            .OrderByDescending<InvoiceAttribute>(invoice => invoice.CreatedAt)
    //                            .Take(1))
    //                    .OrderBy<CustomerAttribute>(customer => customer.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers with optional latest invoice",
    //                MetadataStrategy = MetadataStrategy.Attribute,
    //                ResultType = typeof(LatestInvoicePerCustomerRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerAttribute>(alias: "c")
    //                    .Select<CustomerAttribute>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.FullName
    //                    })
    //                    .OuterApply<CustomerAttribute, InvoiceAttribute>(
    //                        alias: "latest_invoice",
    //                        apply => apply
    //                            .Select<InvoiceAttribute>(invoice => new
    //                            {
    //                                InvoiceId = invoice.Id,
    //                                invoice.CustomerId,
    //                                invoice.Total
    //                            })
    //                            .WhereComputed<InvoiceAttribute, CustomerAttribute>(
    //                                (invoice, customer) => invoice.CustomerId == customer.Id)
    //                            .OrderByDescending<InvoiceAttribute>(invoice => invoice.CreatedAt)
    //                            .Take(1))
    //                    .OrderBy<CustomerAttribute>(customer => customer.Id)
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
    //                Name = "Customers with optional invoices",
    //                MetadataStrategy = MetadataStrategy.EntityFramework,
    //                ResultType = typeof(CustomerOptionalInvoiceRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerEf>(alias: "c")
    //                    .LeftJoin<CustomerEf, InvoiceEf>(
    //                        alias: "i",
    //                        on: (customer, invoice) => customer.Id == invoice.CustomerId)
    //                    .Select<CustomerEf>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.FullName
    //                    })
    //                    .Select<InvoiceEf>(invoice => new
    //                    {
    //                        InvoiceId = invoice.Id,
    //                        invoice.InvoiceNumber,
    //                        invoice.Total
    //                    })
    //                    .OrderBy<CustomerEf>(customer => customer.Id)
    //                    .ThenBy<InvoiceEf>(invoice => invoice.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customer invoices created after registration",
    //                MetadataStrategy = MetadataStrategy.EntityFramework,
    //                ResultType = typeof(CustomerOptionalInvoiceRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerEf>(alias: "c")
    //                    .InnerJoin<CustomerEf, InvoiceEf>(
    //                        alias: "i",
    //                        on: (customer, invoice) =>
    //                            customer.Id == invoice.CustomerId &&
    //                            customer.CreatedAt <= invoice.CreatedAt)
    //                    .Select<CustomerEf>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.FullName
    //                    })
    //                    .Select<InvoiceEf>(invoice => new
    //                    {
    //                        InvoiceId = invoice.Id,
    //                        invoice.InvoiceNumber,
    //                        invoice.Total
    //                    })
    //                    .OrderBy<CustomerEf>(customer => customer.Id)
    //                    .ThenBy<InvoiceEf>(invoice => invoice.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers with latest invoice",
    //                MetadataStrategy = MetadataStrategy.EntityFramework,
    //                ResultType = typeof(LatestInvoicePerCustomerRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerEf>(alias: "c")
    //                    .Select<CustomerEf>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.FullName
    //                    })
    //                    .CrossApply<CustomerEf, InvoiceEf>(
    //                        alias: "latest_invoice",
    //                        apply => apply
    //                            .Select<InvoiceEf>(invoice => new
    //                            {
    //                                InvoiceId = invoice.Id,
    //                                invoice.CustomerId,
    //                                invoice.Total
    //                            })
    //                            .WhereComputed<InvoiceEf, CustomerEf>(
    //                                (invoice, customer) => invoice.CustomerId == customer.Id)
    //                            .OrderByDescending<InvoiceEf>(invoice => invoice.CreatedAt)
    //                            .Take(1))
    //                    .OrderBy<CustomerEf>(customer => customer.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers with optional latest invoice",
    //                MetadataStrategy = MetadataStrategy.EntityFramework,
    //                ResultType = typeof(LatestInvoicePerCustomerRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerEf>(alias: "c")
    //                    .Select<CustomerEf>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.FullName
    //                    })
    //                    .OuterApply<CustomerEf, InvoiceEf>(
    //                        alias: "latest_invoice",
    //                        apply => apply
    //                            .Select<InvoiceEf>(invoice => new
    //                            {
    //                                InvoiceId = invoice.Id,
    //                                invoice.CustomerId,
    //                                invoice.Total
    //                            })
    //                            .WhereComputed<InvoiceEf, CustomerEf>(
    //                                (invoice, customer) => invoice.CustomerId == customer.Id)
    //                            .OrderByDescending<InvoiceEf>(invoice => invoice.CreatedAt)
    //                            .Take(1))
    //                    .OrderBy<CustomerEf>(customer => customer.Id)
    //                    .Build()
    //            }
    //        ];
    //    }
    //}
}
