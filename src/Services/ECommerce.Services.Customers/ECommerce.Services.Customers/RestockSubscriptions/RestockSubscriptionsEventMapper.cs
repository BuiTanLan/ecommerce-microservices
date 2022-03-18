using MicroBootstrap.Abstractions.Core.Domain.Events;
using MicroBootstrap.Abstractions.Core.Domain.Events.External;
using MicroBootstrap.Abstractions.Core.Domain.Events.Internal;

namespace ECommerce.Services.Customers.RestockSubscriptions;

public class RestockSubscriptionsEventMapper : IIntegrationEventMapper
{
    public IReadOnlyList<IIntegrationEvent?> MapToIntegrationEvents(IReadOnlyList<IDomainEvent> domainEvents)
    {
        return new List<IIntegrationEvent?>();
    }

    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent domainEvent)
    {
        return null;
    }
}
