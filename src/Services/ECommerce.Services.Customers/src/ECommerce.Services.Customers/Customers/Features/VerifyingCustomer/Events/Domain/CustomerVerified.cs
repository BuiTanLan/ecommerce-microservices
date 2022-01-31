using BuildingBlocks.Core.Domain.Events.Internal;

namespace ECommerce.Services.Customers.Customers.Features.VerifyingCustomer.Events.Domain;

public record CustomerVerified(long CustomerId) : DomainEvent;
