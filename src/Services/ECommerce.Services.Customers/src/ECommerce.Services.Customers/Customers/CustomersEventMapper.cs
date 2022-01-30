using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Domain.Events.Internal;
using ECommerce.Services.Customers.Customers.Events.Domain;
using ECommerce.Services.Customers.Customers.Features.CompletingCustomer.Events.Domain;

namespace ECommerce.Services.Customers.Customers;

public class CustomersEventMapper : IIntegrationEventMapper
{
    public IReadOnlyList<IIntegrationEvent?> MapToIntegrationEvents(IReadOnlyList<IDomainEvent> domainEvents)
    {
        return domainEvents.Select(MapToIntegrationEvent).ToList();
    }

    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent domainEvent)
    {
        return domainEvent switch
        {
            CustomerLocked e => new Events.Integration.CustomerLocked(e.CustomerId),
            CustomerUnlocked e => new Events.Integration.CustomerUnlocked(e.CustomerId),
            CustomerCompleted e =>
                new Features.CompletingCustomer.Events.Integration.CustomerCompleted(
                    e.CustomerId,
                    e.PhoneNumber,
                    e.Nationality),
            _ => null
        };
    }
}
