Console.WriteLine("========================================");

//var settings = LoadSampleSettings.Create();
//var providers = BuildSampleProviders.Create(settings.ConnectionStrings);

//while (true)
//{
//    Console.Clear();
//    WriteHeader();

//    var provider = SelectProvider(providers);

//    if (provider is null)
//        break;

//    var executor = SelectExecutor();

//    if (executor is null)
//        continue;

//    var metadataStrategy = executor.Name == "EF Core"
//        ? MetadataStrategy.EntityFramework
//        : SelectMetadataStrategy();

//    if (metadataStrategy is null)
//        continue;

//    Console.Clear();

//    Console.WriteLine("========================================");
//    Console.WriteLine($"Provider : {provider.Name}");
//    Console.WriteLine($"Executor : {executor.Name}");
//    Console.WriteLine($"Metadata : {BuildMetadataResolver.GetDisplayName(metadataStrategy.Value)}");
//    Console.WriteLine("========================================");
//    Console.WriteLine();

//    await InitializeDatabaseAsync(provider);
//    await RunSamplesAsync(provider, executor, metadataStrategy.Value);

//    Console.WriteLine();
//    Console.WriteLine("Press any key to continue...");
//    Console.ReadKey();
//}

//static void WriteHeader()
//{
//    Console.WriteLine("========================================");
//    Console.WriteLine("TinyBlueWhale.EngineQuery Samples");
//    Console.WriteLine("========================================");
//    Console.WriteLine();
//}

//static SampleProviderContext? SelectProvider(IReadOnlyList<SampleProviderContext> providers)
//{
//    while (true)
//    {
//        Console.WriteLine("Provider");
//        Console.WriteLine("----------------------------------------");
//        Console.WriteLine("1. SQL Server");
//        Console.WriteLine("2. MySQL");
//        Console.WriteLine("3. PostgreSQL");
//        Console.WriteLine("0. Exit");
//        Console.WriteLine();

//        Console.Write("Select provider: ");

//        var option = Console.ReadLine();

//        Console.WriteLine();

//        switch (option)
//        {
//            case "1":
//                return providers.First(provider => provider.Kind == SampleProviderKind.SqlServer);

//            case "2":
//                return providers.First(provider => provider.Kind == SampleProviderKind.MySql);

//            case "3":
//                return providers.First(provider => provider.Kind == SampleProviderKind.PostgreSql);

//            case "0":
//                return null;

//            default:
//                Console.WriteLine("Invalid option.");
//                Console.WriteLine();
//                break;
//        }
//    }
//}

//static ISampleExecutor? SelectExecutor()
//{
//    while (true)
//    {
//        Console.WriteLine("Executor");
//        Console.WriteLine("----------------------------------------");
//        Console.WriteLine("1. Dapper");
//        Console.WriteLine("2. ADO.NET");
//        Console.WriteLine("3. EF Core");
//        Console.WriteLine("0. Back");
//        Console.WriteLine();

//        Console.Write("Select executor: ");

//        var option = Console.ReadLine();

//        Console.WriteLine();

//        return option switch
//        {
//            "1" => new DapperSampleExecutor(),
//            "2" => new AdoNetSampleExecutor(),
//            "3" => new EntityFrameworkSampleExecutor(),
//            "0" => null,
//            _ => InvalidExecutorOption()
//        };
//    }
//}

//static ISampleExecutor? InvalidExecutorOption()
//{
//    Console.WriteLine("Invalid option.");
//    Console.WriteLine();

//    return null;
//}

//static MetadataStrategy? SelectMetadataStrategy()
//{
//    while (true)
//    {
//        Console.WriteLine("Metadata");
//        Console.WriteLine("----------------------------------------");
//        Console.WriteLine("1. Fluent");
//        Console.WriteLine("2. Attribute");
//        Console.WriteLine("0. Back");
//        Console.WriteLine();

//        Console.Write("Select metadata: ");

//        var option = Console.ReadLine();

//        Console.WriteLine();

//        return option switch
//        {
//            "1" => MetadataStrategy.Fluent,
//            "2" => MetadataStrategy.Attribute,
//            "0" => null,
//            _ => InvalidMetadataOption()
//        };
//    }
//}

//static MetadataStrategy? InvalidMetadataOption()
//{
//    Console.WriteLine("Invalid option.");
//    Console.WriteLine();

//    return null;
//}

//static async Task InitializeDatabaseAsync(SampleProviderContext provider)
//{
//    Console.WriteLine("Initializing database...");
//    Console.WriteLine();

//    var initializer = BuildDatabaseInitializer.Create(provider);

//    await initializer.InitializeAsync(provider);

//    Console.WriteLine("Database initialized.");
//    Console.WriteLine();
//}

//static async Task RunSamplesAsync(
//    SampleProviderContext provider,
//    ISampleExecutor executor,
//    MetadataStrategy metadataStrategy)
//{
//    var scenarios = BuildSalesQueryScenarios.Create(metadataStrategy);

//    foreach (var scenario in scenarios)
//        await RunScenarioAsync(provider, executor, scenario);
//}

//static async Task RunScenarioAsync(
//    SampleProviderContext provider,
//    ISampleExecutor executor,
//    SalesQueryScenario scenario)
//{
//    try
//    {
//        var result = await executor.ExecuteAsync(provider, scenario);

//        WriteSampleResult.Write(result);
//    }
//    catch (Exception exception)
//    {
//        Console.WriteLine("ERROR");
//        Console.WriteLine($"Provider : {provider.Name}");
//        Console.WriteLine($"Executor : {executor.Name}");
//        Console.WriteLine($"Metadata : {BuildMetadataResolver.GetDisplayName(scenario.MetadataStrategy)}");
//        Console.WriteLine($"Scenario : {scenario.Name}");
//        Console.WriteLine($"Message  : {exception.Message}");
//        Console.WriteLine();
//    }
//}
