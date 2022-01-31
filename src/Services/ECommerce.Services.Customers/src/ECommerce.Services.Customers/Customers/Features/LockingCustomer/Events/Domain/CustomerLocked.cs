using BuildingBlocks.Core.Domain.Events.Internal;

namespace ECommerce.Services.Customers.Customers.Features.LockingCustomer.Events.Domain;

public record CustomerLocked(long CustomerId, string? Notes) : DomainEvent;
