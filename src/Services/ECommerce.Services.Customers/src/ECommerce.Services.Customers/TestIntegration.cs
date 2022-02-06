using BuildingBlocks.Core.Domain.Events.External;

namespace ECommerce.Services.Customers;

public record TestIntegration(string Data) : IntegrationEvent;
