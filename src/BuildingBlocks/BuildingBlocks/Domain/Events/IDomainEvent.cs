using MediatR;

namespace BuildingBlocks.Domain.Events;

/// <summary>
/// The domain event interface.
/// </summary>
public interface IDomainEvent : IEvent, INotification
{
}
