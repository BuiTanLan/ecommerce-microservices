using System;
using System.Collections.Generic;
using BuildingBlocks.CQRS.Event;
using MediatR;

namespace BuildingBlocks.Domain;

public interface IDomainEvent : IEvent
{
    DateTime CreatedAt { get; }
    IDictionary<string, object> MetaData { get; }
}
