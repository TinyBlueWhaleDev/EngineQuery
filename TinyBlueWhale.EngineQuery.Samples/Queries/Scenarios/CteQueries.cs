using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Samples.Domain.AttributeMapping;
using TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping;
using TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels;
using TinyBlueWhale.EngineQuery.Samples.Domain.FluentMapping;
using TinyBlueWhale.EngineQuery.Samples.Metadata;

namespace TinyBlueWhale.EngineQuery.Samples.Queries.Scenarios
{
    public static class CteQueries
    {
        public static IReadOnlyList<SalesQueryScenario> CreateForFluent()
        {
            return
            [
                DerivedTableCustomerTotalsFluent(),
            CteCustomerTotalsFluent()
            ];
        }

        public static IReadOnlyList<SalesQueryScenario> CreateForAttribute()
        {
            return
            [
                DerivedTableCustomerTotalsAttribute(),
            CteCustomerTotalsAttribute()
            ];
        }

        public static IReadOnlyList<SalesQueryScenario> CreateForEntityFramework()
        {
            return
            [
                DerivedTableCustomerTotalsEntityFramework(),
            CteCustomerTotalsEntityFramework()
            ];
        }

        private static SalesQueryScenario DerivedTableCustomerTotalsFluent()
        {
            return new SalesQueryScenario
            {
                Name = "Derived table customer totals",
                MetadataStrategy = MetadataStrategy.Fluent,
                ResultType = typeof(CustomerInvoiceTotalRow),
                Build = queryBuilder => queryBuilder
                    .FromSubquery<CustomerInvoiceTotalRow, InvoiceFluent>(
                        alias: "summary",
                        subquery => subquery
                            .From<InvoiceFluent>(alias: "i")
                            .Select<InvoiceFluent>(invoice => new
                            {
                                invoice.CustomerId
                            })
                            .SelectAggregate<InvoiceFluent>(
                                QueryAggregateFunction.Sum,
                                invoice => invoice.Total,
                                "TotalAmount")
                            .GroupBy<InvoiceFluent>(invoice => invoice.CustomerId))
                    .Select<CustomerInvoiceTotalRow>(summary => new
                    {
                        summary.CustomerId,
                        summary.TotalAmount
                    })
                    .WhereComputed<CustomerInvoiceTotalRow>(summary => summary.TotalAmount > 500)
                    .Build()
            };
        }

        private static SalesQueryScenario DerivedTableCustomerTotalsAttribute()
        {
            return new SalesQueryScenario
            {
                Name = "Derived table customer totals",
                MetadataStrategy = MetadataStrategy.Attribute,
                ResultType = typeof(CustomerInvoiceTotalRow),
                Build = queryBuilder => queryBuilder
                    .FromSubquery<CustomerInvoiceTotalRow, InvoiceAttribute>(
                        alias: "summary",
                        subquery => subquery
                            .From<InvoiceAttribute>(alias: "i")
                            .Select<InvoiceAttribute>(invoice => new
                            {
                                invoice.CustomerId
                            })
                            .SelectAggregate<InvoiceAttribute>(
                                QueryAggregateFunction.Sum,
                                invoice => invoice.Total,
                                "TotalAmount")
                            .GroupBy<InvoiceAttribute>(invoice => invoice.CustomerId))
                    .Select<CustomerInvoiceTotalRow>(summary => new
                    {
                        summary.CustomerId,
                        summary.TotalAmount
                    })
                    .WhereComputed<CustomerInvoiceTotalRow>(summary => summary.TotalAmount > 500)
                    .Build()
            };
        }

        private static SalesQueryScenario DerivedTableCustomerTotalsEntityFramework()
        {
            return new SalesQueryScenario
            {
                Name = "Derived table customer totals",
                MetadataStrategy = MetadataStrategy.EntityFramework,
                ResultType = typeof(CustomerInvoiceTotalRow),
                Build = queryBuilder => queryBuilder
                    .FromSubquery<CustomerInvoiceTotalRow, InvoiceEf>(
                        alias: "summary",
                        subquery => subquery
                            .From<InvoiceEf>(alias: "i")
                            .Select<InvoiceEf>(invoice => new
                            {
                                invoice.CustomerId
                            })
                            .SelectAggregate<InvoiceEf>(
                                QueryAggregateFunction.Sum,
                                invoice => invoice.Total,
                                "TotalAmount")
                            .GroupBy<InvoiceEf>(invoice => invoice.CustomerId))
                    .Select<CustomerInvoiceTotalRow>(summary => new
                    {
                        summary.CustomerId,
                        summary.TotalAmount
                    })
                    .WhereComputed<CustomerInvoiceTotalRow>(summary => summary.TotalAmount > 500)
                    .Build()
            };
        }

        private static SalesQueryScenario CteCustomerTotalsFluent()
        {
            return new SalesQueryScenario
            {
                Name = "CTE customer totals",
                MetadataStrategy = MetadataStrategy.Fluent,
                ResultType = typeof(CustomerInvoiceTotalRow),
                Build = queryBuilder => queryBuilder
                    .With<CustomerInvoiceTotalRow, InvoiceFluent>(
                        "customer_totals",
                        cte => cte
                            .From<InvoiceFluent>(alias: "i")
                            .Select<InvoiceFluent>(invoice => new
                            {
                                invoice.CustomerId
                            })
                            .SelectAggregate<InvoiceFluent>(
                                QueryAggregateFunction.Sum,
                                invoice => invoice.Total,
                                "TotalAmount")
                            .GroupBy<InvoiceFluent>(invoice => invoice.CustomerId))
                    .FromCte<CustomerInvoiceTotalRow>("customer_totals")
                    .Select<CustomerInvoiceTotalRow>(summary => new
                    {
                        summary.CustomerId,
                        summary.TotalAmount
                    })
                    .WhereComputed<CustomerInvoiceTotalRow>(summary => summary.TotalAmount > 500)
                    .Build()
            };
        }

        private static SalesQueryScenario CteCustomerTotalsAttribute()
        {
            return new SalesQueryScenario
            {
                Name = "CTE customer totals",
                MetadataStrategy = MetadataStrategy.Attribute,
                ResultType = typeof(CustomerInvoiceTotalRow),
                Build = queryBuilder => queryBuilder
                    .With<CustomerInvoiceTotalRow, InvoiceAttribute>(
                        "customer_totals",
                        cte => cte
                            .From<InvoiceAttribute>(alias: "i")
                            .Select<InvoiceAttribute>(invoice => new
                            {
                                invoice.CustomerId
                            })
                            .SelectAggregate<InvoiceAttribute>(
                                QueryAggregateFunction.Sum,
                                invoice => invoice.Total,
                                "TotalAmount")
                            .GroupBy<InvoiceAttribute>(invoice => invoice.CustomerId))
                    .FromCte<CustomerInvoiceTotalRow>("customer_totals")
                    .Select<CustomerInvoiceTotalRow>(summary => new
                    {
                        summary.CustomerId,
                        summary.TotalAmount
                    })
                    .WhereComputed<CustomerInvoiceTotalRow>(summary => summary.TotalAmount > 500)
                    .Build()
            };
        }

        private static SalesQueryScenario CteCustomerTotalsEntityFramework()
        {
            return new SalesQueryScenario
            {
                Name = "CTE customer totals",
                MetadataStrategy = MetadataStrategy.EntityFramework,
                ResultType = typeof(CustomerInvoiceTotalRow),
                Build = queryBuilder => queryBuilder
                    .With<CustomerInvoiceTotalRow, InvoiceEf>(
                        "customer_totals",
                        cte => cte
                            .From<InvoiceEf>(alias: "i")
                            .Select<InvoiceEf>(invoice => new
                            {
                                invoice.CustomerId
                            })
                            .SelectAggregate<InvoiceEf>(
                                QueryAggregateFunction.Sum,
                                invoice => invoice.Total,
                                "TotalAmount")
                            .GroupBy<InvoiceEf>(invoice => invoice.CustomerId))
                    .FromCte<CustomerInvoiceTotalRow>("customer_totals")
                    .Select<CustomerInvoiceTotalRow>(summary => new
                    {
                        summary.CustomerId,
                        summary.TotalAmount
                    })
                    .WhereComputed<CustomerInvoiceTotalRow>(summary => summary.TotalAmount > 500)
                    .Build()
            };
        }
    }
}
