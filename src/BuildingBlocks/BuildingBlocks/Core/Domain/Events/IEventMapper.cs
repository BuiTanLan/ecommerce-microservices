using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Core.Domain.Model;

namespace BuildingBlocks.Core.Domain.Events;

public interface IEventMapper : IIDomainNotificationEventMapper, IIntegrationEventMapper
{
}

public interface IIDomainNotificationEventMapper
{
    IReadOnlyList<IDomainNotificationEvent> MapToDomainNotificationEvents(IReadOnlyList<IDomainEvent> domainEvents);
    IDomainNotificationEvent MapToDomainNotificationEvent(IDomainEvent domainEvent);
}

public interface IIntegrationEventMapper
{
    IReadOnlyList<IIntegrationEvent> MapToIntegrationEvents(IReadOnlyList<IDomainEvent> domainEvents);
    IIntegrationEvent MapToIntegrationEvent(IDomainEvent domainEvent);
}

public interface IEventMapper<in TAggregate>
    : IIntegrationEventMapper<TAggregate>, IIDomainNotificationEventMapper<TAggregate>
    where TAggregate : IHaveAggregate
{
}

public interface IIntegrationEventMapper<in TAggregate>
    where TAggregate : IHaveAggregate
{
    IReadOnlyList<IIntegrationEvent> MapToIntegrationEvents(IReadOnlyList<IDomainEvent> domainEvents);

    IIntegrationEvent MapToIntegrationEvent(IDomainEvent domainEvent);
}

public interface IIDomainNotificationEventMapper<in TAggregate>
    where TAggregate : IHaveAggregate
{
    IReadOnlyList<IDomainNotificationEvent> MapToDomainNotificationEvents(IReadOnlyList<IDomainEvent> domainEvents);

    IDomainNotificationEvent MapToDomainNotificationEvent(IDomainEvent domainEvent);
}
