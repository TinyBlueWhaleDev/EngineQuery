using TinyBlueWhale.EngineQuery.Samples.Domain.AttributeMapping;
using TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping;
using TinyBlueWhale.EngineQuery.Samples.Domain.FluentMapping;
using TinyBlueWhale.EngineQuery.Samples.Metadata;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels;

namespace TinyBlueWhale.EngineQuery.Samples.Queries.Scenarios
{
    public static class AggregationQueries
    {
        public static IReadOnlyList<SalesQueryScenario> CreateForFluent()
        {
            return
            [
                new SalesQueryScenario
            {
                Name = "Customer invoice summary",
                MetadataStrategy = MetadataStrategy.Fluent,
                ResultType = typeof(CustomerInvoiceSummaryRow),
                Build = queryBuilder => queryBuilder
                    .From<CustomerFluent>(alias: "c")
                    .InnerJoin<CustomerFluent,InvoiceFluent>(alias: "i",on: (customer,invoice) => customer.Id == invoice.CustomerId)
                    .Select<CustomerFluent>(customer => new
                    {
                        CustomerId = customer.Id,
                        customer.FullName
                    })
                    .SelectAggregate<InvoiceFluent>(QueryAggregateFunction.Sum,invoice => invoice.Total,"TotalAmount")
                    .SelectAggregate<InvoiceFluent>(QueryAggregateFunction.Count,invoice => invoice.Id,"InvoiceCount")
                    .GroupBy<CustomerFluent>(customer => new
                    {
                        customer.Id,
                        customer.FullName
                    })
                    .HavingAggregate<InvoiceFluent>(QueryAggregateFunction.Sum,invoice => invoice.Total,QueryComparisonOperator.GreaterThan,100)
                    .Build()
            },
            new SalesQueryScenario
            {
                Name = "Product revenue summary",
                MetadataStrategy = MetadataStrategy.Fluent,
                ResultType = typeof(ProductRevenueSummaryRow),
                Build = queryBuilder => queryBuilder
                    .From<ProductFluent>(alias: "p")
                    .InnerJoin<ProductFluent,InvoiceLineFluent>(alias: "l",on: (product,line) => product.Id == line.ProductId)
                    .Select<ProductFluent>(product => new
                    {
                        ProductId = product.Id,
                        product.Name
                    })
                    .SelectAggregate<InvoiceLineFluent>(QueryAggregateFunction.Sum,line => line.LineTotal,"Revenue")
                    .SelectAggregate<InvoiceLineFluent>(QueryAggregateFunction.Sum,line => line.Quantity,"UnitsSold")
                    .GroupBy<ProductFluent>(product => new
                    {
                        product.Id,
                        product.Name
                    })
                    .HavingAggregate<InvoiceLineFluent>(QueryAggregateFunction.Sum,line => line.LineTotal,QueryComparisonOperator.GreaterThan,100)
                    .Build()
            },
            new SalesQueryScenario
            {
                Name = "Average invoice amount",
                MetadataStrategy = MetadataStrategy.Fluent,
                ResultType = typeof(AverageInvoiceAmountRow),
                Build = queryBuilder => queryBuilder
                    .From<InvoiceFluent>(alias: "i")
                    .SelectAggregate<InvoiceFluent>(QueryAggregateFunction.Average,invoice => invoice.Total,"AverageInvoiceAmount")
                    .Build()
            },
            new SalesQueryScenario
            {
                Name = "Max invoice per customer",
                MetadataStrategy = MetadataStrategy.Fluent,
                ResultType = typeof(MaxInvoicePerCustomerRow),
                Build = queryBuilder => queryBuilder
                    .From<InvoiceFluent>(alias: "i")
                    .Select<InvoiceFluent>(invoice => new
                    {
                        invoice.CustomerId
                    })
                    .SelectAggregate<InvoiceFluent>(QueryAggregateFunction.Maximum,invoice => invoice.Total,"MaxInvoiceTotal")
                    .GroupBy<InvoiceFluent>(invoice => invoice.CustomerId)
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
                Name = "Customer invoice summary",
                MetadataStrategy = MetadataStrategy.Attribute,
                ResultType = typeof(CustomerInvoiceSummaryRow),
                Build = queryBuilder => queryBuilder
                    .From<CustomerAttribute>(alias: "c")
                    .InnerJoin<CustomerAttribute,InvoiceAttribute>(alias: "i",on: (customer,invoice) => customer.Id == invoice.CustomerId)
                    .Select<CustomerAttribute>(customer => new
                    {
                        CustomerId = customer.Id,
                        customer.FullName
                    })
                    .SelectAggregate<InvoiceAttribute>(QueryAggregateFunction.Sum,invoice => invoice.Total,"TotalAmount")
                    .SelectAggregate<InvoiceAttribute>(QueryAggregateFunction.Count,invoice => invoice.Id,"InvoiceCount")
                    .GroupBy<CustomerAttribute>(customer => new
                    {
                        customer.Id,
                        customer.FullName
                    })
                    .HavingAggregate<InvoiceAttribute>(QueryAggregateFunction.Sum,invoice => invoice.Total,QueryComparisonOperator.GreaterThan,100)
                    .Build()
            },
            new SalesQueryScenario
            {
                Name = "Product revenue summary",
                MetadataStrategy = MetadataStrategy.Attribute,
                ResultType = typeof(ProductRevenueSummaryRow),
                Build = queryBuilder => queryBuilder
                    .From<ProductAttribute>(alias: "p")
                    .InnerJoin<ProductAttribute,InvoiceLineAttribute>(alias: "l",on: (product,line) => product.Id == line.ProductId)
                    .Select<ProductAttribute>(product => new
                    {
                        ProductId = product.Id,
                        product.Name
                    })
                    .SelectAggregate<InvoiceLineAttribute>(QueryAggregateFunction.Sum,line => line.LineTotal,"Revenue")
                    .SelectAggregate<InvoiceLineAttribute>(QueryAggregateFunction.Sum,line => line.Quantity,"UnitsSold")
                    .GroupBy<ProductAttribute>(product => new
                    {
                        product.Id,
                        product.Name
                    })
                    .HavingAggregate<InvoiceLineAttribute>(QueryAggregateFunction.Sum,line => line.LineTotal,QueryComparisonOperator.GreaterThan,100)
                    .Build()
            },
            new SalesQueryScenario
            {
                Name = "Average invoice amount",
                MetadataStrategy = MetadataStrategy.Attribute,
                ResultType = typeof(AverageInvoiceAmountRow),
                Build = queryBuilder => queryBuilder
                    .From<InvoiceAttribute>(alias: "i")
                    .SelectAggregate<InvoiceAttribute>(QueryAggregateFunction.Average,invoice => invoice.Total,"AverageInvoiceAmount")
                    .Build()
            },
            new SalesQueryScenario
            {
                Name = "Max invoice per customer",
                MetadataStrategy = MetadataStrategy.Attribute,
                ResultType = typeof(MaxInvoicePerCustomerRow),
                Build = queryBuilder => queryBuilder
                    .From<InvoiceAttribute>(alias: "i")
                    .Select<InvoiceAttribute>(invoice => new
                    {
                        invoice.CustomerId
                    })
                    .SelectAggregate<InvoiceAttribute>(QueryAggregateFunction.Minimum,invoice => invoice.Total,"MaxInvoiceTotal")
                    .GroupBy<InvoiceAttribute>(invoice => invoice.CustomerId)
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
                Name = "Customer invoice summary",
                MetadataStrategy = MetadataStrategy.EntityFramework,
                ResultType = typeof(CustomerInvoiceSummaryRow),
                Build = queryBuilder => queryBuilder
                    .From<CustomerEf>(alias: "c")
                    .InnerJoin<CustomerEf,InvoiceEf>(alias: "i",on: (customer,invoice) => customer.Id == invoice.CustomerId)
                    .Select<CustomerEf>(customer => new
                    {
                        CustomerId = customer.Id,
                        customer.FullName
                    })
                    .SelectAggregate<InvoiceEf>(QueryAggregateFunction.Sum,invoice => invoice.Total,"TotalAmount")
                    .SelectAggregate<InvoiceEf>(QueryAggregateFunction.Count,invoice => invoice.Id,"InvoiceCount")
                    .GroupBy<CustomerEf>(customer => new
                    {
                        customer.Id,
                        customer.FullName
                    })
                    .HavingAggregate<InvoiceEf>(QueryAggregateFunction.Sum,invoice => invoice.Total,QueryComparisonOperator.GreaterThan,100)
                    .Build()
            },
            new SalesQueryScenario
            {
                Name = "Product revenue summary",
                MetadataStrategy = MetadataStrategy.EntityFramework,
                ResultType = typeof(ProductRevenueSummaryRow),
                Build = queryBuilder => queryBuilder
                    .From<ProductEf>(alias: "p")
                    .InnerJoin<ProductEf,InvoiceLineEf>(alias: "l",on: (product,line) => product.Id == line.ProductId)
                    .Select<ProductEf>(product => new
                    {
                        ProductId = product.Id,
                        product.Name
                    })
                    .SelectAggregate<InvoiceLineEf>(QueryAggregateFunction.Sum,line => line.LineTotal,"Revenue")
                    .SelectAggregate<InvoiceLineEf>(QueryAggregateFunction.Sum,line => line.Quantity,"UnitsSold")
                    .GroupBy<ProductEf>(product => new
                    {
                        product.Id,
                        product.Name
                    })
                    .HavingAggregate<InvoiceLineEf>(QueryAggregateFunction.Sum,line => line.LineTotal,QueryComparisonOperator.GreaterThan,100)
                    .Build()
            },
            new SalesQueryScenario
            {
                Name = "Average invoice amount",
                MetadataStrategy = MetadataStrategy.EntityFramework,
                ResultType = typeof(AverageInvoiceAmountRow),
                Build = queryBuilder => queryBuilder
                    .From<InvoiceEf>(alias: "i")
                    .SelectAggregate<InvoiceEf>(QueryAggregateFunction.Average,invoice => invoice.Total,"AverageInvoiceAmount")
                    .Build()
            },
            new SalesQueryScenario
            {
                Name = "Max invoice per customer",
                MetadataStrategy = MetadataStrategy.EntityFramework,
                ResultType = typeof(MaxInvoicePerCustomerRow),
                Build = queryBuilder => queryBuilder
                    .From<InvoiceEf>(alias: "i")
                    .Select<InvoiceEf>(invoice => new
                    {
                        invoice.CustomerId
                    })
                    .SelectAggregate<InvoiceEf>(QueryAggregateFunction.Maximum,invoice => invoice.Total,"MaxInvoiceTotal")
                    .GroupBy<InvoiceEf>(invoice => invoice.CustomerId)
                    .Build()
            }
            ];
        }
    }
}
