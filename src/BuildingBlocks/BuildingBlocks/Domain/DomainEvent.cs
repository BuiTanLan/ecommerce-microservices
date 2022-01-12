using System;
using System.Collections.Generic;

namespace BuildingBlocks.Domain;

public abstract class DomainEvent : IDomainEvent
{
    public string EventType { get { return GetType().FullName; } }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public string CorrelationId { get; init; }
    public IDictionary<string, object> MetaData { get; } = new Dictionary<string, object>();
    public abstract void Flatten();
}
