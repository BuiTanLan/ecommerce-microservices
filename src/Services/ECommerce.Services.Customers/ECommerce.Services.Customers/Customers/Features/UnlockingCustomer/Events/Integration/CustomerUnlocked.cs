using MicroBootstrap.Core.Domain.Events.External;

namespace ECommerce.Services.Customers.Customers.Features.UnlockingCustomer.Events.Integration;

public record CustomerUnlocked(long CustomerId) : IntegrationEvent;
