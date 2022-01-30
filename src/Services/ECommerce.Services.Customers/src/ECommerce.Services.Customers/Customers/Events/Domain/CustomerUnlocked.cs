using BuildingBlocks.Core.Domain.Events.Internal;

namespace ECommerce.Services.Customers.Customers.Events.Domain;

public record CustomerUnlocked(long CustomerId) : DomainEvent;
