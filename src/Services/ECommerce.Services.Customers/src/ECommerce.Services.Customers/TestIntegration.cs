using BuildingBlocks.Abstractions.Domain.Events.External;

namespace ECommerce.Services.Customers;

public record TestIntegration(string Data) : IntegrationEvent;
