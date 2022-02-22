using BuildingBlocks.Abstractions.Domain.Events.External;

namespace ECommerce.Services.Customers.Customers.Features.CompletingCustomer.Events.Integration;

public record CustomerCompleted(
    long CustomerId,
    string PhoneNumber,
    string? Nationality = null) : IntegrationEvent;
