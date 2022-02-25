namespace BuildingBlocks.Abstractions.Persistence.EventStore;

public class StreamEvent
{
    /// <summary>Unique identifier for the event. Uses a sequential Guid</summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The version of the stream this event reflects. The place in the stream.
    /// </summary>
    public int Version { get; init; }

    /// <summary>The actual event data body</summary>
    public string Data { get; init; }

    /// <summary>
    ///     If using Guid's for the stream identity, this will
    ///     refer to the Stream's Id, otherwise it will always be Guid.Empty
    /// </summary>
    public Guid StreamId { get; init; }

    /// <summary>type alias string for the Event type</summary>
    public string EventTypeName { get; init; }

    public DateTimeOffset Timestamp { get; init; }
}
