using BuildingBlocks.Core.Domain.Events.Internal;

namespace ECommerce.Services.Customers.Customers.Events.Domain;

public record CustomerLocked(long CustomerId) : DomainEvent;
