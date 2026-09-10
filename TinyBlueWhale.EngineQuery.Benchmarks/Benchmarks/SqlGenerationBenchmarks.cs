namespace TinyBlueWhale.EngineQuery.Benchmarks.Benchmarks
{
    //[MemoryDiagnoser]
    //public class SqlGenerationBenchmarks
    //{
    //    private FluentEntityMetadataResolver _metadataResolver = null!;

    //    [GlobalSetup]
    //    public void Setup()
    //    {
    //        var registry = new EntityMetadataRegistry();

    //        registry.Entity<BenchmarkCustomer>()
    //            .ToTable("customers")
    //            .Property(customer => customer.Id).HasColumnName("customer_id")
    //            .Property(customer => customer.Email).HasColumnName("email")
    //            .Property(customer => customer.FullName).HasColumnName("full_name")
    //            .Property(customer => customer.IsActive).HasColumnName("is_active")
    //            .Property(customer => customer.CreatedAt).HasColumnName("created_at");

    //        registry.Entity<BenchmarkInvoice>()
    //            .ToTable("invoices")
    //            .Property(invoice => invoice.Id).HasColumnName("invoice_id")
    //            .Property(invoice => invoice.CustomerId).HasColumnName("customer_id")
    //            .Property(invoice => invoice.InvoiceNumber).HasColumnName("invoice_number")
    //            .Property(invoice => invoice.Total).HasColumnName("total")
    //            .Property(invoice => invoice.CreatedAt).HasColumnName("created_at");

    //        _metadataResolver = new FluentEntityMetadataResolver(registry);
    //    }

    //    private QueryBuilder CreateQueryBuilder()
    //    {
    //        return new QueryBuilder(
    //            new SqlServerQueryCompiler(
    //                new SqlServerDatabaseDialect(),
    //                new SqlServerProviderCapabilities()),
    //            _metadataResolver);
    //    }

    //    [Benchmark(Baseline = true)]
    //    public string HandwrittenConstantSql()
    //    {
    //        return """
    //           SELECT [c].[customer_id] AS [CustomerId], [c].[email] AS [Email], [c].[full_name] AS [FullName]
    //           FROM [customers] AS [c]
    //           WHERE ([c].[is_active] = @p0)
    //           ORDER BY [c].[created_at] DESC
    //           OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY
    //           """;
    //    }

    //    [Benchmark]
    //    public string HandwrittenStringBuilderSql()
    //    {
    //        var builder = new StringBuilder();

    //        builder.Append("SELECT ");
    //        builder.Append("[c].[customer_id] AS [CustomerId], ");
    //        builder.Append("[c].[email] AS [Email], ");
    //        builder.Append("[c].[full_name] AS [FullName]");
    //        builder.AppendLine();
    //        builder.Append("FROM [customers] AS [c]");
    //        builder.AppendLine();
    //        builder.Append("WHERE ([c].[is_active] = @p0)");
    //        builder.AppendLine();
    //        builder.Append("ORDER BY [c].[created_at] DESC");
    //        builder.AppendLine();
    //        builder.Append("OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY");

    //        return builder.ToString();
    //    }

    //    [Benchmark]
    //    public object EngineQuery_BasicSelect()
    //    {
    //        return CreateQueryBuilder()
    //            .From<BenchmarkCustomer>(alias: "c")
    //            .Select<BenchmarkCustomer>(customer => new
    //            {
    //                CustomerId = customer.Id,
    //                Email = customer.Email,
    //                FullName = customer.FullName
    //            })
    //            .Where<BenchmarkCustomer>(customer => customer.IsActive)
    //            .OrderByDescending<BenchmarkCustomer>(customer => customer.CreatedAt)
    //            .Skip(0)
    //            .Take(10)
    //            .Build();
    //    }

    //    [Benchmark]
    //    public object EngineQuery_JoinAggregate()
    //    {
    //        return CreateQueryBuilder()
    //            .From<BenchmarkCustomer>(alias: "c")
    //            .InnerJoin<BenchmarkCustomer, BenchmarkInvoice>(
    //                alias: "i",
    //                on: (customer, invoice) => customer.Id == invoice.CustomerId)
    //            .Select<BenchmarkCustomer>(customer => new
    //            {
    //                CustomerId = customer.Id,
    //                FullName = customer.FullName
    //            })
    //            .SelectAggregate<BenchmarkInvoice>(
    //                QueryAggregateFunction.Sum,
    //                invoice => invoice.Total,
    //                "TotalAmount")
    //            .SelectAggregate<BenchmarkInvoice>(
    //                QueryAggregateFunction.Count,
    //                invoice => invoice.Id,
    //                "InvoiceCount")
    //            .GroupBy<BenchmarkCustomer>(customer => new
    //            {
    //                customer.Id,
    //                customer.FullName
    //            })
    //            .HavingAggregate<BenchmarkInvoice>(
    //                QueryAggregateFunction.Sum,
    //                invoice => invoice.Total,
    //                QueryComparisonOperator.GreaterThan,
    //                100)
    //            .Build();
    //    }

    //    [Benchmark]
    //    public object EngineQuery_ExistsSubquery()
    //    {
    //        return CreateQueryBuilder()
    //            .From<BenchmarkCustomer>(alias: "c")
    //            .Select<BenchmarkCustomer>(customer => new
    //            {
    //                CustomerId = customer.Id,
    //                Email = customer.Email
    //            })
    //            .WhereExists<BenchmarkCustomer, BenchmarkInvoice>(
    //                alias: "i",
    //                subquery => subquery
    //                    .WhereComputed<BenchmarkInvoice, BenchmarkCustomer>(
    //                        (invoice, customer) =>
    //                            invoice.CustomerId == customer.Id &&
    //                            invoice.Total > 500))
    //            .Build();
    //    }

    //    [Benchmark]
    //    public object EngineQuery_DerivedTable()
    //    {
    //        return CreateQueryBuilder()
    //            .FromSubquery<CustomerInvoiceTotal, BenchmarkInvoice>(
    //                alias: "summary",
    //                subquery => subquery
    //                    .From<BenchmarkInvoice>(alias: "i")
    //                    .Select<BenchmarkInvoice>(invoice => new
    //                    {
    //                        invoice.CustomerId
    //                    })
    //                    .SelectAggregate<BenchmarkInvoice>(
    //                        QueryAggregateFunction.Sum,
    //                        invoice => invoice.Total,
    //                        "TotalAmount")
    //                    .GroupBy<BenchmarkInvoice>(invoice => invoice.CustomerId))
    //            .Select<CustomerInvoiceTotal>(summary => new
    //            {
    //                summary.CustomerId,
    //                summary.TotalAmount
    //            })
    //            .WhereComputed<CustomerInvoiceTotal>(summary => summary.TotalAmount > 500)
    //            .Build();
    //    }

    //    [Benchmark]
    //    public object EngineQuery_Cte()
    //    {
    //        return CreateQueryBuilder()
    //            .With<CustomerInvoiceTotal, BenchmarkInvoice>(
    //                "customer_totals",
    //                cte => cte
    //                    .From<BenchmarkInvoice>(alias: "i")
    //                    .Select<BenchmarkInvoice>(invoice => new
    //                    {
    //                        invoice.CustomerId
    //                    })
    //                    .SelectAggregate<BenchmarkInvoice>(
    //                        QueryAggregateFunction.Sum,
    //                        invoice => invoice.Total,
    //                        "TotalAmount")
    //                    .GroupBy<BenchmarkInvoice>(invoice => invoice.CustomerId))
    //            .FromCte<CustomerInvoiceTotal>("customer_totals")
    //            .Select<CustomerInvoiceTotal>(summary => new
    //            {
    //                summary.CustomerId,
    //                summary.TotalAmount
    //            })
    //            .WhereComputed<CustomerInvoiceTotal>(summary => summary.TotalAmount > 500)
    //            .Build();
    //    }

    //    [Benchmark]
    //    public object EngineQuery_WindowFunction()
    //    {
    //        return CreateQueryBuilder()
    //            .From<BenchmarkInvoice>(alias: "i")
    //            .Select<BenchmarkInvoice>(invoice => new
    //            {
    //                InvoiceId = invoice.Id,
    //                CustomerId = invoice.CustomerId,
    //                Total = invoice.Total
    //            })
    //            .SelectRowNumber(
    //                "CustomerInvoiceRank",
    //                window => window
    //                    .PartitionBy<BenchmarkInvoice>(invoice => invoice.CustomerId)
    //                    .OrderByDescending<BenchmarkInvoice>(invoice => invoice.Total))
    //            .Build();
    //    }

    //    [Benchmark]
    //    public object EngineQuery_SetOperation()
    //    {
    //        return CreateQueryBuilder()
    //            .From<BenchmarkCustomer>(alias: "c")
    //            .Select<BenchmarkCustomer>(customer => new
    //            {
    //                Email = customer.Email
    //            })
    //            .Where<BenchmarkCustomer>(customer => customer.IsActive)
    //            .UnionAll<BenchmarkCustomer>(set => set
    //                .From<BenchmarkCustomer>(alias: "c2")
    //                .Select<BenchmarkCustomer>(customer => new
    //                {
    //                    Email = customer.Email
    //                })
    //                .Where<BenchmarkCustomer>(customer => !customer.IsActive))
    //            .Build();
    //    }
    //}
}
