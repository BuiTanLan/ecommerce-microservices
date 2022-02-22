using BuildingBlocks.Abstractions.Domain.Events.External;

namespace ECommerce.Services.Customers.Customers.Features.CreatingCustomer.Events.Integration;

public record CustomerCreated(long CustomerId) : IntegrationEvent;
