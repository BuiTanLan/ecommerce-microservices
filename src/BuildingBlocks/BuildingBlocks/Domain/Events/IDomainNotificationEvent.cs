using MediatR;

namespace BuildingBlocks.Domain.Events;

public interface IDomainNotificationEvent : IEvent, INotification
{
}
