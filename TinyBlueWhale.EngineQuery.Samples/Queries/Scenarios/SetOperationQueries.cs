namespace TinyBlueWhale.EngineQuery.Samples.Queries.Scenarios
{
    //public static class SetOperationQueries
    //{
    //    public static IReadOnlyList<SalesQueryScenario> CreateForFluent()
    //    {
    //        return
    //        [
    //            new SalesQueryScenario
    //        {
    //            Name = "Active inactive customer union all",
    //            MetadataStrategy = MetadataStrategy.Fluent,
    //            ResultType = typeof(CustomerEmailRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<CustomerFluent>(alias: "c")
    //                .Select<CustomerFluent>(customer => new
    //                {
    //                    customer.Email
    //                })
    //                .Where<CustomerFluent>(customer => customer.IsActive)
    //                .UnionAll<CustomerFluent>(set => set
    //                    .From<CustomerFluent>(alias: "c2")
    //                    .Select<CustomerFluent>(customer => new
    //                    {
    //                        customer.Email
    //                    })
    //                    .Where<CustomerFluent>(customer => !customer.IsActive))
    //                .Build()
    //        },
    //        new SalesQueryScenario
    //        {
    //            Name = "Active customer intersect",
    //            MetadataStrategy = MetadataStrategy.Fluent,
    //            ResultType = typeof(CustomerEmailRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<CustomerFluent>(alias: "c")
    //                .Select<CustomerFluent>(customer => new
    //                {
    //                    customer.Email
    //                })
    //                .Where<CustomerFluent>(customer => customer.IsActive)
    //                .Intersect<CustomerFluent>(set => set
    //                    .From<CustomerFluent>(alias: "c2")
    //                    .Select<CustomerFluent>(customer => new
    //                    {
    //                        customer.Email
    //                    })
    //                    .Where<CustomerFluent>(customer => customer.Email.Contains("@test.com")))
    //                .Build()
    //        },
    //        new SalesQueryScenario
    //        {
    //            Name = "Active customer except inactive",
    //            MetadataStrategy = MetadataStrategy.Fluent,
    //            ResultType = typeof(CustomerEmailRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<CustomerFluent>(alias: "c")
    //                .Select<CustomerFluent>(customer => new
    //                {
    //                    customer.Email
    //                })
    //                .Where<CustomerFluent>(customer => customer.IsActive)
    //                .Except<CustomerFluent>(set => set
    //                    .From<CustomerFluent>(alias: "c2")
    //                    .Select<CustomerFluent>(customer => new
    //                    {
    //                        customer.Email
    //                    })
    //                    .Where<CustomerFluent>(customer => !customer.IsActive))
    //                .Build()
    //        }
    //        ];
    //    }

    //    public static IReadOnlyList<SalesQueryScenario> CreateForAttribute()
    //    {
    //        return
    //        [
    //            new SalesQueryScenario
    //        {
    //            Name = "Active inactive customer union all",
    //            MetadataStrategy = MetadataStrategy.Attribute,
    //            ResultType = typeof(CustomerEmailRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<CustomerAttribute>(alias: "c")
    //                .Select<CustomerAttribute>(customer => new
    //                {
    //                    customer.Email
    //                })
    //                .Where<CustomerAttribute>(customer => customer.IsActive)
    //                .UnionAll<CustomerAttribute>(set => set
    //                    .From<CustomerAttribute>(alias: "c2")
    //                    .Select<CustomerAttribute>(customer => new
    //                    {
    //                        customer.Email
    //                    })
    //                    .Where<CustomerAttribute>(customer => !customer.IsActive))
    //                .Build()
    //        },
    //        new SalesQueryScenario
    //        {
    //            Name = "Active customer intersect",
    //            MetadataStrategy = MetadataStrategy.Attribute,
    //            ResultType = typeof(CustomerEmailRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<CustomerAttribute>(alias: "c")
    //                .Select<CustomerAttribute>(customer => new
    //                {
    //                    customer.Email
    //                })
    //                .Where<CustomerAttribute>(customer => customer.IsActive)
    //                .Intersect<CustomerAttribute>(set => set
    //                    .From<CustomerAttribute>(alias: "c2")
    //                    .Select<CustomerAttribute>(customer => new
    //                    {
    //                        customer.Email
    //                    })
    //                    .Where<CustomerAttribute>(customer => customer.Email.Contains("@test.com")))
    //                .Build()
    //        },
    //        new SalesQueryScenario
    //        {
    //            Name = "Active customer except inactive",
    //            MetadataStrategy = MetadataStrategy.Attribute,
    //            ResultType = typeof(CustomerEmailRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<CustomerAttribute>(alias: "c")
    //                .Select<CustomerAttribute>(customer => new
    //                {
    //                    customer.Email
    //                })
    //                .Where<CustomerAttribute>(customer => customer.IsActive)
    //                .Except<CustomerAttribute>(set => set
    //                    .From<CustomerAttribute>(alias: "c2")
    //                    .Select<CustomerAttribute>(customer => new
    //                    {
    //                        customer.Email
    //                    })
    //                    .Where<CustomerAttribute>(customer => !customer.IsActive))
    //                .Build()
    //        }
    //        ];
    //    }

    //    public static IReadOnlyList<SalesQueryScenario> CreateForEntityFramework()
    //    {
    //        return
    //        [
    //            new SalesQueryScenario
    //        {
    //            Name = "Active inactive customer union all",
    //            MetadataStrategy = MetadataStrategy.EntityFramework,
    //            ResultType = typeof(CustomerEmailRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<CustomerEf>(alias: "c")
    //                .Select<CustomerEf>(customer => new
    //                {
    //                    customer.Email
    //                })
    //                .Where<CustomerEf>(customer => customer.IsActive)
    //                .UnionAll<CustomerEf>(set => set
    //                    .From<CustomerEf>(alias: "c2")
    //                    .Select<CustomerEf>(customer => new
    //                    {
    //                        customer.Email
    //                    })
    //                    .Where<CustomerEf>(customer => !customer.IsActive))
    //                .Build()
    //        },
    //        new SalesQueryScenario
    //        {
    //            Name = "Active customer intersect",
    //            MetadataStrategy = MetadataStrategy.EntityFramework,
    //            ResultType = typeof(CustomerEmailRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<CustomerEf>(alias: "c")
    //                .Select<CustomerEf>(customer => new
    //                {
    //                    customer.Email
    //                })
    //                .Where<CustomerEf>(customer => customer.IsActive)
    //                .Intersect<CustomerEf>(set => set
    //                    .From<CustomerEf>(alias: "c2")
    //                    .Select<CustomerEf>(customer => new
    //                    {
    //                        customer.Email
    //                    })
    //                    .Where<CustomerEf>(customer => customer.Email.Contains("@test.com")))
    //                .Build()
    //        },
    //        new SalesQueryScenario
    //        {
    //            Name = "Active customer except inactive",
    //            MetadataStrategy = MetadataStrategy.EntityFramework,
    //            ResultType = typeof(CustomerEmailRow),
    //            Build = queryBuilder => queryBuilder
    //                .From<CustomerEf>(alias: "c")
    //                .Select<CustomerEf>(customer => new
    //                {
    //                    customer.Email
    //                })
    //                .Where<CustomerEf>(customer => customer.IsActive)
    //                .Except<CustomerEf>(set => set
    //                    .From<CustomerEf>(alias: "c2")
    //                    .Select<CustomerEf>(customer => new
    //                    {
    //                        customer.Email
    //                    })
    //                    .Where<CustomerEf>(customer => !customer.IsActive))
    //                .Build()
    //        }
    //        ];
    //    }
    //}
}
