using BuildingBlocks.Core.Domain.Events.External;

namespace ECommerce.Services.Customers.Customers.Features.LockingCustomer.Events.Integration;

public record CustomerLocked(long CustomerId) : IntegrationEvent;
