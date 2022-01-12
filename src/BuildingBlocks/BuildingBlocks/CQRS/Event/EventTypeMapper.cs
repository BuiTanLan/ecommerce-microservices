using System;
using System.Collections.Concurrent;
using BuildingBlocks.Utils;

namespace BuildingBlocks.CQRS.Event;

public class EventTypeMapper
{
    private static readonly EventTypeMapper s_instance = new();

    private readonly ConcurrentDictionary<Type, string> _typeNameMap = new();
    private readonly ConcurrentDictionary<string, Type> _typeMap = new();

    public static void AddCustomMap<T>(string mappedEventTypeName) => AddCustomMap(typeof(T), mappedEventTypeName);

    public static void AddCustomMap(Type eventType, string mappedEventTypeName)
    {
        s_instance._typeNameMap.AddOrUpdate(eventType, mappedEventTypeName, (_, _) => mappedEventTypeName);
        s_instance._typeMap.AddOrUpdate(mappedEventTypeName, eventType, (_, _) => eventType);
    }

    public static string ToName<TEventType>() => ToName(typeof(TEventType));

    public static string ToName(Type eventType) => s_instance._typeNameMap.GetOrAdd(eventType, _ =>
    {
        var eventTypeName = eventType.FullName!.Replace(".", "_");

        s_instance._typeMap.AddOrUpdate(eventTypeName, eventType, (_, _) => eventType);

        return eventTypeName;
    });

    public static Type ToType(string eventTypeName) => s_instance._typeMap.GetOrAdd(eventTypeName, _ =>
    {
        var type = TypeProvider.GetFirstMatchingTypeFromCurrentDomainAssembly(eventTypeName.Replace("_", "."))!;

        if (type == null)
            throw new System.Exception($"Type map for '{eventTypeName}' wasn't found!");

        s_instance._typeNameMap.AddOrUpdate(type, eventTypeName, (_, _) => eventTypeName);

        return type;
    });
}
