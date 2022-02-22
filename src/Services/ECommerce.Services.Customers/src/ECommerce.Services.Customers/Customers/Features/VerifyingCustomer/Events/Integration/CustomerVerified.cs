using BuildingBlocks.Abstractions.Domain.Events.External;

namespace ECommerce.Services.Customers.Customers.Features.VerifyingCustomer.Events.Integration;

public record CustomerVerified(long CustomerId) : IntegrationEvent;
