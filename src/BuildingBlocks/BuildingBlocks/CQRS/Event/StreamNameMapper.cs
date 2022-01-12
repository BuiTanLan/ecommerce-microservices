using System;
using System.Collections.Concurrent;
using System.Linq;

namespace BuildingBlocks.CQRS.Event;

public class StreamNameMapper
{
    private static readonly StreamNameMapper s_instance = new();

    private readonly ConcurrentDictionary<Type, string> _typeNameMap = new();

    public static void AddCustomMap<TStream>(string mappedStreamName) =>
        AddCustomMap(typeof(TStream), mappedStreamName);

    public static void AddCustomMap(Type streamType, string mappedStreamName)
    {
        s_instance._typeNameMap.AddOrUpdate(streamType, mappedStreamName, (_, _) => mappedStreamName);
    }

    public static string ToStreamPrefix<TStream>() => ToStreamPrefix(typeof(TStream));

    public static string ToStreamPrefix(Type streamType) => s_instance._typeNameMap.GetOrAdd(streamType, (_) =>
    {
        var modulePrefix = streamType.Namespace!.Split(".").First();
        return $"{modulePrefix}_{streamType.Name}";
    });

    public static string ToStreamId<TStream>(object aggregateId, object? tenantId = null) =>
        ToStreamId(typeof(TStream), aggregateId);

    public static string ToStreamId(Type streamType, object aggregateId, object? tenantId = null)
    {
        var tenantPrefix = tenantId != null ? $"{tenantId}_" : "";

        return $"{tenantPrefix}{ToStreamPrefix(streamType)}-{aggregateId}";
    }
}
