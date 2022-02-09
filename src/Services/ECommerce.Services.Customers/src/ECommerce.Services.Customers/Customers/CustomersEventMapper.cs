using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Domain.Events.Internal;
using ECommerce.Services.Customers.Customers.Features.CompletingCustomer.Events.Domain;
using ECommerce.Services.Customers.Customers.Features.CreatingCustomer.Events.Domain;
using ECommerce.Services.Customers.Customers.Features.LockingCustomer.Events.Domain;
using ECommerce.Services.Customers.Customers.Features.UnlockingCustomer.Events.Domain;
using ECommerce.Services.Customers.Customers.Features.VerifyingCustomer.Events.Domain;

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
            TestDomainEvent e => new TestIntegration(e.Data),
            CustomerCreated e => new Features.CreatingCustomer.Events.Integration
                .CustomerCreated(e.Customer.Id),
            CustomerLocked e => new Features.LockingCustomer.Events.Integration.CustomerLocked(e.CustomerId),
            CustomerUnlocked e => new Features.UnlockingCustomer.Events.Integration.CustomerUnlocked(e.CustomerId),
            CustomerCompleted e =>
                new Features.CompletingCustomer.Events.Integration.CustomerCompleted(
                    e.Customer.Id,
                    e.Customer.PhoneNumber!.Value,
                    e.Customer.Nationality),
            CustomerVerified e => new Features.VerifyingCustomer.Events.Integration.CustomerVerified(e.CustomerId),
            _ => null
        };
    }
}
