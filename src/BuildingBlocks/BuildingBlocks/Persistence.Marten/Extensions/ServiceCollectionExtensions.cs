using BuildingBlocks.Abstractions.Domain.Events;
using BuildingBlocks.Abstractions.Persistence.Marten;
using BuildingBlocks.Core.Extensions.DependencyInjection;
using BuildingBlocks.Core.IdsGenerator;
using BuildingBlocks.Core.Threading;
using BuildingBlocks.Persistence.Marten.Repositories;
using BuildingBlocks.Web.Extensions;
using BuildingBlocks.Web.Extensions.ServiceCollectionExtensions;
using Marten;
using Marten.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;

namespace BuildingBlocks.Persistence.Marten.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomMarten(
        this IServiceCollection services,
        Func<IServiceProvider, string> connectionStringConfigurator,
        Action<StoreOptions>? setAdditionalOptions = null,
        string? schemaName = null
    )
    {
        services.AddScoped(
            sp => CreateDocumentStore(connectionStringConfigurator(sp), setAdditionalOptions, schemaName));

        services.Add(
            sp =>
            {
                var store = sp.GetRequiredService<DocumentStore>();
                return CreateDocumentSession(store);
            });

        services.AddScoped<IDomainEventsAccessor, MartenDomainEventAccessor>();
        services.AddEventStore<MartenEventStore>();

        services.AddTransient(typeof(IMartenRepository<,>), typeof(MartenDocumentRepository<,>));
        services.AddTransient(typeof(IMartenEventSourcedRepository<>), typeof(MartenEventSourcedRepository<>));
        services.AddTransient(typeof(IMartenUnitOfWork), typeof(MartenUnitOfWork));
        services.AddScoped<IIdGenerator<Guid>, MartenIdGenerator>();

        return services;
    }

    public static IServiceCollection AddCustomMarten(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<StoreOptions>? configureOptions = null
    )
    {
        var martenOptions = configuration.GetOptions<MartenOptions>(nameof(MartenOptions));

        var documentStore = services
            .AddMarten(options =>
            {
                SetStoreOptions(options, martenOptions, configureOptions);
            })
            .InitializeStore();

        SetupSchema(documentStore, martenOptions, 1);

        services.AddScoped<IDomainEventsAccessor, MartenDomainEventAccessor>();

        services.AddEventStore<MartenEventStore>();
        services.AddTransient(typeof(IMartenRepository<,>), typeof(MartenDocumentRepository<,>));
        services.AddTransient(typeof(IMartenEventSourcedRepository<>), typeof(MartenEventSourcedRepository<>));
        services.AddTransient(typeof(IMartenUnitOfWork), typeof(MartenUnitOfWork));
        services.AddScoped<IIdGenerator<Guid>, MartenIdGenerator>();

        return services;
    }

    private static void SetupSchema(IDocumentStore documentStore, MartenOptions martenOptions, int retryLeft = 1)
    {
        try
        {
            if (martenOptions.ShouldRecreateDatabase)
                documentStore.Advanced.Clean.CompletelyRemoveAll();

            using (NoSynchronizationContextScope.Enter())
            {
                documentStore.Schema.ApplyAllConfiguredChangesToDatabaseAsync().Wait();
            }
        }
        catch
        {
            if (retryLeft == 0) throw;

            Thread.Sleep(1000);
            SetupSchema(documentStore, martenOptions, --retryLeft);
        }
    }

    private static void SetStoreOptions(
        StoreOptions options,
        MartenOptions martenOptions,
        Action<StoreOptions>? configureOptions = null)
    {
        options.Connection(martenOptions.ConnectionString);
        options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;

        var schemaName = Environment.GetEnvironmentVariable("SchemaName");
        options.Events.DatabaseSchemaName = schemaName ?? martenOptions.WriteModelSchema;
        options.DatabaseSchemaName = schemaName ?? martenOptions.ReadModelSchema;

        options.UseDefaultSerialization(
            nonPublicMembersStorage: NonPublicMembersStorage.NonPublicSetters,
            enumStorage: EnumStorage.AsString);

        // options.Projections.AsyncMode = config.DaemonMode;

        configureOptions?.Invoke(options);
    }

    public static DocumentStore CreateDocumentStore(
        string connectionString,
        Action<StoreOptions>? setAdditionalOptions = null,
        string? moduleName = null)
    {
        var store = DocumentStore.For(_ =>
        {
            _.Connection(connectionString);
            _.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;

            if (string.IsNullOrEmpty(moduleName) == false)
                _.DatabaseSchemaName = _.Events.DatabaseSchemaName = moduleName.ToLower();

            setAdditionalOptions?.Invoke(_);
        });

        return store;
    }

    public static IDocumentSession CreateDocumentSession(DocumentStore store)
    {
        var session = store.OpenSession(SessionOptions.ForCurrentTransaction());
        return session;
    }
}
