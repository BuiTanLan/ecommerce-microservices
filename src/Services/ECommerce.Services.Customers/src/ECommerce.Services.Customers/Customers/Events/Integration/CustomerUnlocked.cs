using BuildingBlocks.Core.Domain.Events.External;

namespace ECommerce.Services.Customers.Customers.Events.Integration;

public record CustomerUnlocked(long CustomerId) : IntegrationEvent;
