using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Persistence.EventStoreDB.Repository;
using BuildingBlocks.Persistence.EventStoreDB.Subscriptions;
using BuildingBlocks.Web.Extensions;
using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Persistence.EventStoreDB;

public static class Extensions
{
    public static IServiceCollection AddEventStore(
        this IServiceCollection services,
        IConfiguration configuration,
        EventStoreDbOptions? options = null)
    {
        var eventStoreDbConfig = configuration.GetOptions<EventStoreDbOptions>(nameof(EventStoreDbOptions));

        services.AddSingleton(
            new EventStoreClient(EventStoreClientSettings.Create(eventStoreDbConfig.ConnectionString)));

        services.AddTransient(typeof(IEventSourcedRepository<>), typeof(EventStoreDbRepository<>));

        if (options?.UseInternalCheckpointing != false)
        {
            services
                .AddTransient<ISubscriptionCheckpointRepository, EventStoreDbSubscriptionCheckPointRepository>();
        }

        return services;
    }

    public static IServiceCollection AddEventStoreDbSubscriptionToAll(
        this IServiceCollection services,
        string subscriptionId,
        SubscriptionFilterOptions? filterOptions = null,
        Action<EventStoreClientOperationOptions>? configureOperation = null,
        UserCredentials? credentials = null,
        bool checkpointToEventStoreDb = true)
    {
        if (checkpointToEventStoreDb)
        {
            services
                .AddTransient<ISubscriptionCheckpointRepository, EventStoreDbSubscriptionCheckPointRepository>();
        }

        return services.AddHostedService(serviceProvider =>
            new SubscribeToAllBackgroundWorker(
                serviceProvider,
                serviceProvider.GetRequiredService<EventStoreClient>(),
                serviceProvider.GetRequiredService<ISubscriptionCheckpointRepository>(),
                serviceProvider.GetRequiredService<ILogger<SubscribeToAllBackgroundWorker>>(),
                subscriptionId,
                filterOptions,
                configureOperation,
                credentials
            )
        );
    }
}
