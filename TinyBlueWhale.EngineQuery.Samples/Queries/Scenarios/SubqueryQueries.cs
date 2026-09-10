namespace TinyBlueWhale.EngineQuery.Samples.Queries.Scenarios
{
    //public static class SubqueryQueries
    //{
    //    public static IReadOnlyList<SalesQueryScenario> CreateForFluent()
    //    {
    //        return
    //        [
    //            new SalesQueryScenario
    //            {
    //                Name = "Active customers when high value invoices exist",
    //                MetadataStrategy = MetadataStrategy.Fluent,
    //                ResultType = typeof(CustomerLookupRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerFluent>(alias: "c")
    //                    .Select<CustomerFluent>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.Email
    //                    })
    //                    .Where<CustomerFluent>(customer => customer.IsActive)
    //                    .WhereExists<InvoiceFluent>(
    //                        subquery => subquery
    //                            .From<InvoiceFluent>(alias: "global_invoice")
    //                            .Where<InvoiceFluent>(invoice => invoice.Total > 1000))
    //                    .OrderBy<CustomerFluent>(customer => customer.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers with high value invoices",
    //                MetadataStrategy = MetadataStrategy.Fluent,
    //                ResultType = typeof(CustomerLookupRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerFluent>(alias: "c")
    //                    .Select<CustomerFluent>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.Email
    //                    })
    //                    .WhereExists<CustomerFluent, InvoiceFluent>(
    //                        alias: "i",
    //                        subquery => subquery
    //                            .WhereComputed<InvoiceFluent, CustomerFluent>(
    //                                (invoice, customer) =>
    //                                    invoice.CustomerId == customer.Id &&
    //                                    invoice.Total > 500))
    //                    .OrderBy<CustomerFluent>(customer => customer.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers without invoices",
    //                MetadataStrategy = MetadataStrategy.Fluent,
    //                ResultType = typeof(CustomerLookupRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerFluent>(alias: "c")
    //                    .Select<CustomerFluent>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.Email
    //                    })
    //                    .WhereNotExists<CustomerFluent, InvoiceFluent>(
    //                        alias: "i",
    //                        subquery => subquery
    //                            .WhereComputed<InvoiceFluent, CustomerFluent>(
    //                                (invoice, customer) =>
    //                                    invoice.CustomerId == customer.Id))
    //                    .OrderBy<CustomerFluent>(customer => customer.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers in high value invoice subquery",
    //                MetadataStrategy = MetadataStrategy.Fluent,
    //                ResultType = typeof(CustomerLookupRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerFluent>(alias: "c")
    //                    .Select<CustomerFluent>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.Email
    //                    })
    //                    .WhereIn<CustomerFluent, InvoiceFluent>(
    //                        customer => customer.Id,
    //                        alias: "i",
    //                        subquery => subquery
    //                            .Select<InvoiceFluent>(invoice => new
    //                            {
    //                                invoice.CustomerId
    //                            })
    //                            .Where<InvoiceFluent>(invoice => invoice.Total > 500))
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
    //                Name = "Active customers when high value invoices exist",
    //                MetadataStrategy = MetadataStrategy.Attribute,
    //                ResultType = typeof(CustomerLookupRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerAttribute>(alias: "c")
    //                    .Select<CustomerAttribute>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.Email
    //                    })
    //                    .Where<CustomerAttribute>(customer => customer.IsActive)
    //                    .WhereExists<InvoiceAttribute>(
    //                        subquery => subquery
    //                            .From<InvoiceAttribute>(alias: "global_invoice")
    //                            .Where<InvoiceAttribute>(invoice => invoice.Total > 1000))
    //                    .OrderBy<CustomerAttribute>(customer => customer.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers with high value invoices",
    //                MetadataStrategy = MetadataStrategy.Attribute,
    //                ResultType = typeof(CustomerLookupRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerAttribute>(alias: "c")
    //                    .Select<CustomerAttribute>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.Email
    //                    })
    //                    .WhereExists<CustomerAttribute, InvoiceAttribute>(
    //                        alias: "i",
    //                        subquery => subquery
    //                            .WhereComputed<InvoiceAttribute, CustomerAttribute>(
    //                                (invoice, customer) =>
    //                                    invoice.CustomerId == customer.Id &&
    //                                    invoice.Total > 500))
    //                    .OrderBy<CustomerAttribute>(customer => customer.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers without invoices",
    //                MetadataStrategy = MetadataStrategy.Attribute,
    //                ResultType = typeof(CustomerLookupRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerAttribute>(alias: "c")
    //                    .Select<CustomerAttribute>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.Email
    //                    })
    //                    .WhereNotExists<CustomerAttribute, InvoiceAttribute>(
    //                        alias: "i",
    //                        subquery => subquery
    //                            .WhereComputed<InvoiceAttribute, CustomerAttribute>(
    //                                (invoice, customer) =>
    //                                    invoice.CustomerId == customer.Id))
    //                    .OrderBy<CustomerAttribute>(customer => customer.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers in high value invoice subquery",
    //                MetadataStrategy = MetadataStrategy.Attribute,
    //                ResultType = typeof(CustomerLookupRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerAttribute>(alias: "c")
    //                    .Select<CustomerAttribute>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.Email
    //                    })
    //                    .WhereIn<CustomerAttribute, InvoiceAttribute>(
    //                        customer => customer.Id,
    //                        alias: "i",
    //                        subquery => subquery
    //                            .Select<InvoiceAttribute>(invoice => new
    //                            {
    //                                invoice.CustomerId
    //                            })
    //                            .Where<InvoiceAttribute>(invoice => invoice.Total > 500))
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
    //                Name = "Active customers when high value invoices exist",
    //                MetadataStrategy = MetadataStrategy.EntityFramework,
    //                ResultType = typeof(CustomerLookupRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerEf>(alias: "c")
    //                    .Select<CustomerEf>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.Email
    //                    })
    //                    .Where<CustomerEf>(customer => customer.IsActive)
    //                    .WhereExists<InvoiceEf>(
    //                        subquery => subquery
    //                            .From<InvoiceEf>(alias: "global_invoice")
    //                            .Where<InvoiceEf>(invoice => invoice.Total > 1000))
    //                    .OrderBy<CustomerEf>(customer => customer.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers with high value invoices",
    //                MetadataStrategy = MetadataStrategy.EntityFramework,
    //                ResultType = typeof(CustomerLookupRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerEf>(alias: "c")
    //                    .Select<CustomerEf>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.Email
    //                    })
    //                    .WhereExists<CustomerEf, InvoiceEf>(
    //                        alias: "i",
    //                        subquery => subquery
    //                            .WhereComputed<InvoiceEf, CustomerEf>(
    //                                (invoice, customer) =>
    //                                    invoice.CustomerId == customer.Id &&
    //                                    invoice.Total > 500))
    //                    .OrderBy<CustomerEf>(customer => customer.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers without invoices",
    //                MetadataStrategy = MetadataStrategy.EntityFramework,
    //                ResultType = typeof(CustomerLookupRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerEf>(alias: "c")
    //                    .Select<CustomerEf>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.Email
    //                    })
    //                    .WhereNotExists<CustomerEf, InvoiceEf>(
    //                        alias: "i",
    //                        subquery => subquery
    //                            .WhereComputed<InvoiceEf, CustomerEf>(
    //                                (invoice, customer) =>
    //                                    invoice.CustomerId == customer.Id))
    //                    .OrderBy<CustomerEf>(customer => customer.Id)
    //                    .Build()
    //            },
    //            new SalesQueryScenario
    //            {
    //                Name = "Customers in high value invoice subquery",
    //                MetadataStrategy = MetadataStrategy.EntityFramework,
    //                ResultType = typeof(CustomerLookupRow),
    //                Build = queryBuilder => queryBuilder
    //                    .From<CustomerEf>(alias: "c")
    //                    .Select<CustomerEf>(customer => new
    //                    {
    //                        CustomerId = customer.Id,
    //                        customer.Email
    //                    })
    //                    .WhereIn<CustomerEf, InvoiceEf>(
    //                        customer => customer.Id,
    //                        alias: "i",
    //                        subquery => subquery
    //                            .Select<InvoiceEf>(invoice => new
    //                            {
    //                                invoice.CustomerId
    //                            })
    //                            .Where<InvoiceEf>(invoice => invoice.Total > 500))
    //                    .OrderBy<CustomerEf>(customer => customer.Id)
    //                    .Build()
    //            }
    //        ];
    //    }
    //}
}
