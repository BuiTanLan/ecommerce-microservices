using System.Reflection;
using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Utils;
using BuildingBlocks.Utils.Reflections;

namespace BuildingBlocks.Core.Extensions;

public static class EventsExtensions
{
    public static IEnumerable<Type> GetHandledIntegrationEventTypes(this Assembly[] assemblies)
    {
        var messageHandlerTypes = typeof(IEventHandler<>).GetAllTypesImplementingOpenGenericInterface(assemblies)
            .ToList();

        var inheritsTypes = messageHandlerTypes.SelectMany(x => x.GetInterfaces())
            .Where(x => x.GetInterfaces().Any(i => i.IsGenericType) &&
                        x.GetGenericTypeDefinition() == typeof(IEventHandler<>));

        foreach (var inheritsType in inheritsTypes)
        {
            var messageType = inheritsType.GetGenericArguments().First();
            if (messageType.IsAssignableTo(typeof(IIntegrationEvent)))
            {
                yield return messageType;
            }
        }
    }

    public static IEnumerable<Type> GetHandledDomainNotificationEventTypes(this Assembly[] assemblies)
    {
        var messageHandlerTypes = typeof(IEventHandler<>).GetAllTypesImplementingOpenGenericInterface(assemblies)
            .ToList();

        var inheritsTypes = messageHandlerTypes.SelectMany(x => x.GetInterfaces())
            .Where(x => x.GetInterfaces().Any(i => i.IsGenericType) &&
                        x.GetGenericTypeDefinition() == typeof(IEventHandler<>));

        foreach (var inheritsType in inheritsTypes)
        {
            var messageType = inheritsType.GetGenericArguments().First();
            if (messageType.IsAssignableTo(typeof(IDomainNotificationEvent)))
            {
                yield return messageType;
            }
        }
    }

    public static IEnumerable<Type> GetHandledDomainEventTypes(this Assembly[] assemblies)
    {
        var messageHandlerTypes = typeof(IEventHandler<>).GetAllTypesImplementingOpenGenericInterface(assemblies)
            .ToList();

        var inheritsTypes = messageHandlerTypes.SelectMany(x => x.GetInterfaces())
            .Where(x => x.GetInterfaces().Any(i => i.IsGenericType) &&
                        x.GetGenericTypeDefinition() == typeof(IEventHandler<>));

        foreach (var inheritsType in inheritsTypes)
        {
            var messageType = inheritsType.GetGenericArguments().First();
            if (messageType.IsAssignableTo(typeof(IDomainEvent)))
            {
                yield return messageType;
            }
        }
    }

    public static IEnumerable<IDomainNotificationEvent> GetDomainNotificationEventsFromDomainEvents(
        this IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (IDomainEvent domainEvent in domainEvents.Where(x =>
                     typeof(IHaveNotificationEvent).IsAssignableFrom(x.GetType())))
        {
            Type genericType = typeof(DomainNotificationEventWrapper<>)
                .MakeGenericType(domainEvent.GetType());

            IDomainNotificationEvent domainNotificationEvent = (IDomainNotificationEvent)Activator
                .CreateInstance(genericType, domainEvent);

            yield return domainNotificationEvent;
        }
    }

    public static IEnumerable<IIntegrationEvent> GetIntegrationEventsFromDomainEvents(
        this IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (IDomainEvent domainEvent in domainEvents.Where(x =>
                     typeof(IHaveExternalEvent).IsAssignableFrom(x.GetType())))
        {
            Type genericType = typeof(IntegrationEventWrapper<>)
                .MakeGenericType(domainEvent.GetType());

            IIntegrationEvent domainNotificationEvent = (IIntegrationEvent)Activator
                .CreateInstance(genericType, domainEvent);

            yield return domainNotificationEvent;
        }
    }
}
