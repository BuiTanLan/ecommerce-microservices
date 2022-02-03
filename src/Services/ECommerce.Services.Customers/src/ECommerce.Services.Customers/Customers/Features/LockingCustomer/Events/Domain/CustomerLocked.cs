using BuildingBlocks.Core.Domain.Events.Internal;
using ECommerce.Services.Customers.Customers.ValueObjects;

namespace ECommerce.Services.Customers.Customers.Features.LockingCustomer.Events.Domain;

public record CustomerLocked(CustomerId CustomerId, string? Notes) : DomainEvent;
