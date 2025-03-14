using ECommerce.Services.Customers.Customers.Features.CompletingCustomer.Events.Domain;
using ECommerce.Services.Customers.Customers.Features.CreatingCustomer.Events.Domain;
using ECommerce.Services.Customers.Customers.Features.LockingCustomer.Events.Domain;
using ECommerce.Services.Customers.Customers.Features.UnlockingCustomer.Events.Domain;
using ECommerce.Services.Customers.Customers.Features.VerifyingCustomer.Events.Domain;
using MicroBootstrap.Abstractions.Core.Domain.Events;
using MicroBootstrap.Abstractions.Core.Domain.Events.External;
using MicroBootstrap.Abstractions.Core.Domain.Events.Internal;

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
            CustomerLocked e => new Features.LockingCustomer.Events.Integration.CustomerLocked(e.Customer.Id),
            CustomerUnlocked e => new Features.UnlockingCustomer.Events.Integration.CustomerUnlocked(e.Customer.Id),
            CustomerCompleted e =>
                new Features.CompletingCustomer.Events.Integration.CustomerCompleted(
                    e.Customer.Id,
                    e.Customer.PhoneNumber!.Value,
                    e.Customer.Nationality),
            CustomerVerified e => new Features.VerifyingCustomer.Events.Integration.CustomerVerified(e.Customer.Id),
            _ => null
        };
    }
}
