using System.Reflection;
using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Utils;

namespace BuildingBlocks.Core.Extensions;

public static class AssemblyExtensions
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
}
