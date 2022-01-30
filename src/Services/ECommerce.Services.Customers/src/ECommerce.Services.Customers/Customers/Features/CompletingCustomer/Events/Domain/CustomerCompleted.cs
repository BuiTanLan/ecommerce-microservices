using BuildingBlocks.Core.Domain.Events.Internal;

namespace ECommerce.Services.Customers.Customers.Features.CompletingCustomer.Events.Domain;

public record CustomerCompleted(
    long CustomerId,
    string PhoneNumber,
    DateTime? BirthDate = null,
    string? Country = null,
    string? City = null,
    string? DetailAddress = null,
    string? Nationality = null) : DomainEvent;
