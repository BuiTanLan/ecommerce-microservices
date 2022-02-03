using BuildingBlocks.Core.Domain.Events.Internal;
using ECommerce.Services.Customers.Customers.ValueObjects;

namespace ECommerce.Services.Customers.Customers.Features.VerifyingCustomer.Events.Domain;

public record CustomerVerified(CustomerId CustomerId) : DomainEvent;
