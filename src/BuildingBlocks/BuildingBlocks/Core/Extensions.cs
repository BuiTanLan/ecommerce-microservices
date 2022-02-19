using System.Reflection;
using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Core.Objects;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, params Assembly[]? assembliesToScan)
    {
        var systemInfo = SystemInfo.New();
        services.AddSingleton<ISystemInfo>(systemInfo);
        services.AddSingleton(systemInfo);
        services.AddScoped<IEventProcessor, EventProcessor>();
        services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();
        services.AddScoped<IDomainNotificationEventPublisher, DomainNotificationEventPublisher>();

        RegisterEventMappers(services, assembliesToScan);

        return services;
    }

    private static void RegisterEventMappers(IServiceCollection services, params Assembly[]? assembliesToScan)
    {
        services.Scan(scan => scan
            .FromAssemblies(assembliesToScan ?? AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(classes => classes.AssignableTo(typeof(IEventMapper)), false)
            .AddClasses(classes => classes.AssignableTo(typeof(IIntegrationEventMapper)), false)
            .AddClasses(classes => classes.AssignableTo(typeof(IIDomainNotificationEventMapper)), false)
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }
}
