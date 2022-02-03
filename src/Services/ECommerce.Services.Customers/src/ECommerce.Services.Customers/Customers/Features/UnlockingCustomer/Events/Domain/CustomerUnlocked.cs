using BuildingBlocks.Core.Domain.Events.Internal;
using ECommerce.Services.Customers.Customers.ValueObjects;

namespace ECommerce.Services.Customers.Customers.Features.UnlockingCustomer.Events.Domain;

public record CustomerUnlocked(CustomerId CustomerId) : DomainEvent;
