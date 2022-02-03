using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Core.Domain.ValueObjects;
using ECommerce.Services.Customers.Customers.ValueObjects;

namespace ECommerce.Services.Customers.Customers.Features.CompletingCustomer.Events.Domain;

public record CustomerCompleted(
    CustomerId CustomerId,
    PhoneNumber PhoneNumber,
    BirthDate? BirthDate = null,
    Address? Address = null,
    Nationality? Nationality = null) : DomainEvent;
