using BuildingBlocks.Core.Domain.Events.Internal;

namespace ECommerce.Services.Customers.Customers.Features.UnlockingCustomer.Events.Domain;

public record CustomerUnlocked(long CustomerId) : DomainEvent;
