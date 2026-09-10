namespace TinyBlueWhale.EngineQuery.Samples.Queries.Scenarios
{
    //public static class WindowQueries
    //{
    //    public static IReadOnlyList<SalesQueryScenario> CreateForFluent()
    //    {
    //        return
    //        [
    //            InvoiceRowNumberRankingFluent(),
    //        InvoiceRankDenseRankFluent(),
    //        InvoiceLagLeadComparisonFluent(),
    //        InvoiceFirstLastValuesFluent(),
    //        InvoiceQuartileNtileFluent()
    //        ];
    //    }

    //    public static IReadOnlyList<SalesQueryScenario> CreateForAttribute()
    //    {
    //        return
    //        [
    //            InvoiceRowNumberRankingAttribute(),
    //        InvoiceRankDenseRankAttribute(),
    //        InvoiceLagLeadComparisonAttribute(),
    //        InvoiceFirstLastValuesAttribute(),
    //        InvoiceQuartileNtileAttribute()
    //        ];
    //    }

    //    public static IReadOnlyList<SalesQueryScenario> CreateForEntityFramework()
    //    {
    //        return
    //        [
    //            InvoiceRowNumberRankingEntityFramework(),
    //        InvoiceRankDenseRankEntityFramework(),
    //        InvoiceLagLeadComparisonEntityFramework(),
    //        InvoiceFirstLastValuesEntityFramework(),
    //        InvoiceQuartileNtileEntityFramework()
    //        ];
    //    }

    //    private static SalesQueryScenario InvoiceRowNumberRankingFluent()
    //    {
    //        return new SalesQueryScenario
    //        {
    //            Name = "Invoice row number ranking",
    //            MetadataStrategy = MetadataStrategy.Fluent,
    //            ResultType = typeof(InvoiceRankingRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<InvoiceFluent>(alias: "i")
    //                .Select<InvoiceFluent>(invoice => new
    //                {
    //                    InvoiceId = invoice.Id,
    //                    invoice.CustomerId,
    //                    invoice.Total
    //                })
    //                .SelectRowNumber(
    //                    "CustomerInvoiceRank",
    //                    window => window
    //                        .PartitionBy<InvoiceFluent>(invoice => invoice.CustomerId)
    //                        .OrderByDescending<InvoiceFluent>(invoice => invoice.Total))
    //                .Build()
    //        };
    //    }

    //    private static SalesQueryScenario InvoiceRowNumberRankingAttribute()
    //    {
    //        return new SalesQueryScenario
    //        {
    //            Name = "Invoice row number ranking",
    //            MetadataStrategy = MetadataStrategy.Attribute,
    //            ResultType = typeof(InvoiceRankingRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<InvoiceAttribute>(alias: "i")
    //                .Select<InvoiceAttribute>(invoice => new
    //                {
    //                    InvoiceId = invoice.Id,
    //                    invoice.CustomerId,
    //                    invoice.Total
    //                })
    //                .SelectRowNumber(
    //                    "CustomerInvoiceRank",
    //                    window => window
    //                        .PartitionBy<InvoiceAttribute>(invoice => invoice.CustomerId)
    //                        .OrderByDescending<InvoiceAttribute>(invoice => invoice.Total))
    //                .Build()
    //        };
    //    }

    //    private static SalesQueryScenario InvoiceRowNumberRankingEntityFramework()
    //    {
    //        return new SalesQueryScenario
    //        {
    //            Name = "Invoice row number ranking",
    //            MetadataStrategy = MetadataStrategy.EntityFramework,
    //            ResultType = typeof(InvoiceRankingRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<InvoiceEf>(alias: "i")
    //                .Select<InvoiceEf>(invoice => new
    //                {
    //                    InvoiceId = invoice.Id,
    //                    invoice.CustomerId,
    //                    invoice.Total
    //                })
    //                .SelectRowNumber(
    //                    "CustomerInvoiceRank",
    //                    window => window
    //                        .PartitionBy<InvoiceEf>(invoice => invoice.CustomerId)
    //                        .OrderByDescending<InvoiceEf>(invoice => invoice.Total))
    //                .Build()
    //        };
    //    }

    //    private static SalesQueryScenario InvoiceRankDenseRankFluent()
    //    {
    //        return new SalesQueryScenario
    //        {
    //            Name = "Invoice rank dense rank",
    //            MetadataStrategy = MetadataStrategy.Fluent,
    //            ResultType = typeof(InvoiceRankDenseRankRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<InvoiceFluent>(alias: "i")
    //                .Select<InvoiceFluent>(invoice => new
    //                {
    //                    InvoiceId = invoice.Id,
    //                    invoice.CustomerId,
    //                    invoice.Total
    //                })
    //                .SelectRank(
    //                    "InvoiceRank",
    //                    window => window
    //                        .PartitionBy<InvoiceFluent>(invoice => invoice.CustomerId)
    //                        .OrderByDescending<InvoiceFluent>(invoice => invoice.Total))
    //                .SelectDenseRank(
    //                    "DenseInvoiceRank",
    //                    window => window
    //                        .PartitionBy<InvoiceFluent>(invoice => invoice.CustomerId)
    //                        .OrderByDescending<InvoiceFluent>(invoice => invoice.Total))
    //                .Build()
    //        };
    //    }

    //    private static SalesQueryScenario InvoiceRankDenseRankAttribute()
    //    {
    //        return new SalesQueryScenario
    //        {
    //            Name = "Invoice rank dense rank",
    //            MetadataStrategy = MetadataStrategy.Attribute,
    //            ResultType = typeof(InvoiceRankDenseRankRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<InvoiceAttribute>(alias: "i")
    //                .Select<InvoiceAttribute>(invoice => new
    //                {
    //                    InvoiceId = invoice.Id,
    //                    invoice.CustomerId,
    //                    invoice.Total
    //                })
    //                .SelectRank(
    //                    "InvoiceRank",
    //                    window => window
    //                        .PartitionBy<InvoiceAttribute>(invoice => invoice.CustomerId)
    //                        .OrderByDescending<InvoiceAttribute>(invoice => invoice.Total))
    //                .SelectDenseRank(
    //                    "DenseInvoiceRank",
    //                    window => window
    //                        .PartitionBy<InvoiceAttribute>(invoice => invoice.CustomerId)
    //                        .OrderByDescending<InvoiceAttribute>(invoice => invoice.Total))
    //                .Build()
    //        };
    //    }

    //    private static SalesQueryScenario InvoiceRankDenseRankEntityFramework()
    //    {
    //        return new SalesQueryScenario
    //        {
    //            Name = "Invoice rank dense rank",
    //            MetadataStrategy = MetadataStrategy.EntityFramework,
    //            ResultType = typeof(InvoiceRankDenseRankRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<InvoiceEf>(alias: "i")
    //                .Select<InvoiceEf>(invoice => new
    //                {
    //                    InvoiceId = invoice.Id,
    //                    invoice.CustomerId,
    //                    invoice.Total
    //                })
    //                .SelectRank(
    //                    "InvoiceRank",
    //                    window => window
    //                        .PartitionBy<InvoiceEf>(invoice => invoice.CustomerId)
    //                        .OrderByDescending<InvoiceEf>(invoice => invoice.Total))
    //                .SelectDenseRank(
    //                    "DenseInvoiceRank",
    //                    window => window
    //                        .PartitionBy<InvoiceEf>(invoice => invoice.CustomerId)
    //                        .OrderByDescending<InvoiceEf>(invoice => invoice.Total))
    //                .Build()
    //        };
    //    }

    //    private static SalesQueryScenario InvoiceLagLeadComparisonFluent()
    //    {
    //        return new SalesQueryScenario
    //        {
    //            Name = "Invoice lag lead comparison",
    //            MetadataStrategy = MetadataStrategy.Fluent,
    //            ResultType = typeof(InvoiceLagLeadRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<InvoiceFluent>(alias: "i")
    //                .Select<InvoiceFluent>(invoice => new
    //                {
    //                    InvoiceId = invoice.Id,
    //                    invoice.CustomerId,
    //                    invoice.Total
    //                })
    //                .SelectLag<InvoiceFluent>(
    //                    invoice => invoice.Total,
    //                    "PreviousInvoiceTotal",
    //                    window => window
    //                        .PartitionBy<InvoiceFluent>(invoice => invoice.CustomerId)
    //                        .OrderBy<InvoiceFluent>(invoice => invoice.CreatedAt))
    //                .SelectLead<InvoiceFluent>(
    //                    invoice => invoice.Total,
    //                    "NextInvoiceTotal",
    //                    window => window
    //                        .PartitionBy<InvoiceFluent>(invoice => invoice.CustomerId)
    //                        .OrderBy<InvoiceFluent>(invoice => invoice.CreatedAt))
    //                .Build()
    //        };
    //    }

    //    private static SalesQueryScenario InvoiceLagLeadComparisonAttribute()
    //    {
    //        return new SalesQueryScenario
    //        {
    //            Name = "Invoice lag lead comparison",
    //            MetadataStrategy = MetadataStrategy.Attribute,
    //            ResultType = typeof(InvoiceLagLeadRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<InvoiceAttribute>(alias: "i")
    //                .Select<InvoiceAttribute>(invoice => new
    //                {
    //                    InvoiceId = invoice.Id,
    //                    invoice.CustomerId,
    //                    invoice.Total
    //                })
    //                .SelectLag<InvoiceAttribute>(
    //                    invoice => invoice.Total,
    //                    "PreviousInvoiceTotal",
    //                    window => window
    //                        .PartitionBy<InvoiceAttribute>(invoice => invoice.CustomerId)
    //                        .OrderBy<InvoiceAttribute>(invoice => invoice.CreatedAt))
    //                .SelectLead<InvoiceAttribute>(
    //                    invoice => invoice.Total,
    //                    "NextInvoiceTotal",
    //                    window => window
    //                        .PartitionBy<InvoiceAttribute>(invoice => invoice.CustomerId)
    //                        .OrderBy<InvoiceAttribute>(invoice => invoice.CreatedAt))
    //                .Build()
    //        };
    //    }

    //    private static SalesQueryScenario InvoiceLagLeadComparisonEntityFramework()
    //    {
    //        return new SalesQueryScenario
    //        {
    //            Name = "Invoice lag lead comparison",
    //            MetadataStrategy = MetadataStrategy.EntityFramework,
    //            ResultType = typeof(InvoiceLagLeadRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<InvoiceEf>(alias: "i")
    //                .Select<InvoiceEf>(invoice => new
    //                {
    //                    InvoiceId = invoice.Id,
    //                    invoice.CustomerId,
    //                    invoice.Total
    //                })
    //                .SelectLag<InvoiceEf>(
    //                    invoice => invoice.Total,
    //                    "PreviousInvoiceTotal",
    //                    window => window
    //                        .PartitionBy<InvoiceEf>(invoice => invoice.CustomerId)
    //                        .OrderBy<InvoiceEf>(invoice => invoice.CreatedAt))
    //                .SelectLead<InvoiceEf>(
    //                    invoice => invoice.Total,
    //                    "NextInvoiceTotal",
    //                    window => window
    //                        .PartitionBy<InvoiceEf>(invoice => invoice.CustomerId)
    //                        .OrderBy<InvoiceEf>(invoice => invoice.CreatedAt))
    //                .Build()
    //        };
    //    }

    //    private static SalesQueryScenario InvoiceFirstLastValuesFluent()
    //    {
    //        return new SalesQueryScenario
    //        {
    //            Name = "Invoice first last values",
    //            MetadataStrategy = MetadataStrategy.Fluent,
    //            ResultType = typeof(InvoiceFirstLastValueRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<InvoiceFluent>(alias: "i")
    //                .Select<InvoiceFluent>(invoice => new
    //                {
    //                    InvoiceId = invoice.Id,
    //                    invoice.CustomerId,
    //                    invoice.Total
    //                })
    //                .SelectFirstValue<InvoiceFluent>(
    //                    invoice => invoice.Total,
    //                    "FirstInvoiceTotal",
    //                    window => window
    //                        .PartitionBy<InvoiceFluent>(invoice => invoice.CustomerId)
    //                        .OrderBy<InvoiceFluent>(invoice => invoice.CreatedAt))
    //                .SelectLastValue<InvoiceFluent>(
    //                    invoice => invoice.Total,
    //                    "LastInvoiceTotal",
    //                    window => window
    //                        .PartitionBy<InvoiceFluent>(invoice => invoice.CustomerId)
    //                        .OrderBy<InvoiceFluent>(invoice => invoice.CreatedAt))
    //                .Build()
    //        };
    //    }

    //    private static SalesQueryScenario InvoiceFirstLastValuesAttribute()
    //    {
    //        return new SalesQueryScenario
    //        {
    //            Name = "Invoice first last values",
    //            MetadataStrategy = MetadataStrategy.Attribute,
    //            ResultType = typeof(InvoiceFirstLastValueRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<InvoiceAttribute>(alias: "i")
    //                .Select<InvoiceAttribute>(invoice => new
    //                {
    //                    InvoiceId = invoice.Id,
    //                    invoice.CustomerId,
    //                    invoice.Total
    //                })
    //                .SelectFirstValue<InvoiceAttribute>(
    //                    invoice => invoice.Total,
    //                    "FirstInvoiceTotal",
    //                    window => window
    //                        .PartitionBy<InvoiceAttribute>(invoice => invoice.CustomerId)
    //                        .OrderBy<InvoiceAttribute>(invoice => invoice.CreatedAt))
    //                .SelectLastValue<InvoiceAttribute>(
    //                    invoice => invoice.Total,
    //                    "LastInvoiceTotal",
    //                    window => window
    //                        .PartitionBy<InvoiceAttribute>(invoice => invoice.CustomerId)
    //                        .OrderBy<InvoiceAttribute>(invoice => invoice.CreatedAt))
    //                .Build()
    //        };
    //    }

    //    private static SalesQueryScenario InvoiceFirstLastValuesEntityFramework()
    //    {
    //        return new SalesQueryScenario
    //        {
    //            Name = "Invoice first last values",
    //            MetadataStrategy = MetadataStrategy.EntityFramework,
    //            ResultType = typeof(InvoiceFirstLastValueRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<InvoiceEf>(alias: "i")
    //                .Select<InvoiceEf>(invoice => new
    //                {
    //                    InvoiceId = invoice.Id,
    //                    invoice.CustomerId,
    //                    invoice.Total
    //                })
    //                .SelectFirstValue<InvoiceEf>(
    //                    invoice => invoice.Total,
    //                    "FirstInvoiceTotal",
    //                    window => window
    //                        .PartitionBy<InvoiceEf>(invoice => invoice.CustomerId)
    //                        .OrderBy<InvoiceEf>(invoice => invoice.CreatedAt))
    //                .SelectLastValue<InvoiceEf>(
    //                    invoice => invoice.Total,
    //                    "LastInvoiceTotal",
    //                    window => window
    //                        .PartitionBy<InvoiceEf>(invoice => invoice.CustomerId)
    //                        .OrderBy<InvoiceEf>(invoice => invoice.CreatedAt))
    //                .Build()
    //        };
    //    }

    //    private static SalesQueryScenario InvoiceQuartileNtileFluent()
    //    {
    //        return new SalesQueryScenario
    //        {
    //            Name = "Invoice quartile ntile",
    //            MetadataStrategy = MetadataStrategy.Fluent,
    //            ResultType = typeof(InvoiceQuartileRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<InvoiceFluent>(alias: "i")
    //                .Select<InvoiceFluent>(invoice => new
    //                {
    //                    InvoiceId = invoice.Id,
    //                    invoice.CustomerId,
    //                    invoice.Total
    //                })
    //                .SelectNtile(
    //                    4,
    //                    "InvoiceQuartile",
    //                    window => window.OrderByDescending<InvoiceFluent>(invoice => invoice.Total))
    //                .Build()
    //        };
    //    }

    //    private static SalesQueryScenario InvoiceQuartileNtileAttribute()
    //    {
    //        return new SalesQueryScenario
    //        {
    //            Name = "Invoice quartile ntile",
    //            MetadataStrategy = MetadataStrategy.Attribute,
    //            ResultType = typeof(InvoiceQuartileRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<InvoiceAttribute>(alias: "i")
    //                .Select<InvoiceAttribute>(invoice => new
    //                {
    //                    InvoiceId = invoice.Id,
    //                    invoice.CustomerId,
    //                    invoice.Total
    //                })
    //                .SelectNtile(
    //                    4,
    //                    "InvoiceQuartile",
    //                    window => window
    //                        .OrderByDescending<InvoiceAttribute>(invoice => invoice.Total))
    //                .Build()
    //        };
    //    }

    //    private static SalesQueryScenario InvoiceQuartileNtileEntityFramework()
    //    {
    //        return new SalesQueryScenario
    //        {
    //            Name = "Invoice quartile ntile",
    //            MetadataStrategy = MetadataStrategy.EntityFramework,
    //            ResultType = typeof(InvoiceQuartileRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<InvoiceEf>(alias: "i")
    //                .Select<InvoiceEf>(invoice => new
    //                {
    //                    InvoiceId = invoice.Id,
    //                    invoice.CustomerId,
    //                    invoice.Total
    //                })
    //                .SelectNtile(
    //                    4,
    //                    "InvoiceQuartile",
    //                    window => window.OrderByDescending<InvoiceEf>(invoice => invoice.Total))
    //                .Build()
    //        };
    //    }
    //}
}
