using System.Reflection;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Objects;
using BuildingBlocks.EFCore;
using BuildingBlocks.Messaging.BackgroundServices;
using BuildingBlocks.Messaging.Message;
using BuildingBlocks.Messaging.Outbox;
using BuildingBlocks.Messaging.Outbox.EF;
using BuildingBlocks.Messaging.Outbox.InMemory;
using BuildingBlocks.Messaging.Scheduling;
using BuildingBlocks.Messaging.Serialization;
using BuildingBlocks.Messaging.Serialization.Newtonsoft;
using BuildingBlocks.Utils;
using BuildingBlocks.Utils.Reflections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging;

public enum TxOutboxConstants
{
    InMemory = 1,
    EntityFramework = 2
}

public static class Extensions
{
    public static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        TxOutboxConstants outboxProvider = TxOutboxConstants.EntityFramework)
    {
        switch (outboxProvider)
        {
            case TxOutboxConstants.InMemory:
            {
                services.AddSingleton<IInMemoryOutboxStore, InMemoryOutboxStore>();
                services.AddScoped<IOutboxService, InMemoryOutboxService>();
                break;
            }

            case TxOutboxConstants.EntityFramework:
            {
                var outboxOption = Guard.Against.Null(configuration.GetOutboxOptions(), nameof(OutboxOptions));
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

                services.AddDbContext<OutboxDataContext>(options =>
                {
                    options.UseNpgsql(outboxOption.ConnectionString, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(OutboxDataContext).Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    }).UseSnakeCaseNamingConvention();
                });

                services.AddUnitOfWork<OutboxDataContext>();
                services.AddScoped<IOutboxDataContext>(provider => provider.GetRequiredService<OutboxDataContext>());
                services.AddScoped<IOutboxService, EfOutboxService<OutboxDataContext>>();
                services.AddScoped<IMessagesExecutor, MessagesExecutor>();

                break;
            }
        }

        services.AddSingleton<NewtonsoftJsonUnSupportedTypeMatcher>();
        services.AddSingleton<IMessageSerializer, NewtonsoftJsonMessageSerializer>();

        services.AddScoped<IMessageDispatcher, MessageDispatcher>();

        services.AddHostedService<SubscribersBackgroundService>();
        services.AddHostedService<ConsumerBackgroundWorker>();
        services.AddHostedService<OutboxProcessorBackgroundService>();

        var typeResolver = new TypeResolver();
        services.AddSingleton<ITypeResolver>(typeResolver);
        RegisterIntegrationMessagesToTypeResolver(typeResolver);

        return services;
    }

    private static void RegisterIntegrationMessagesToTypeResolver(
        ITypeResolver typeResolver)
    {
        Console.WriteLine("preloading all message types...");

        var messageType = typeof(IIntegrationEvent);

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(x => x.GetTypes())
            .Where(type => messageType.IsAssignableFrom(type) && type.IsInterface == false && type.IsAbstract == false)
            .Distinct()
            .ToList();

        typeResolver.Register(types);

        Console.WriteLine("preloading all message types completed!");
    }

    public static IEnumerable<Type> GetHandledMessageTypes(params Assembly[] assemblies)
    {
        var messageHandlerTypes = typeof(IMessageHandler<>).GetAllTypesImplementingOpenGenericInterface(assemblies)
            .ToList();

        var inheritsTypes = messageHandlerTypes.SelectMany(x => x.GetInterfaces())
            .Where(x => x.GetInterfaces().Any(i => i.IsGenericType) &&
                        x.GetGenericTypeDefinition() == typeof(IMessageHandler<>));

        foreach (var inheritsType in inheritsTypes)
        {
            var messageType = inheritsType.GetGenericArguments().First();
            if (messageType.IsAssignableTo(typeof(IMessage)))
            {
                yield return messageType;
            }
        }
    }

    public static IServiceCollection AddBusSubscriber(this IServiceCollection services, Type subscriberType)
    {
        if (services.All(s => s.ImplementationType != subscriberType))
            services.AddSingleton(typeof(IBusSubscriber), subscriberType);
        return services;
    }
}
