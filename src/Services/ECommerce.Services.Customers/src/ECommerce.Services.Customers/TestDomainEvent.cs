using MicroBootstrap.Core.Domain.Events.Internal;

namespace ECommerce.Services.Customers;

public record TestDomainEvent(string Data) : DomainEvent
{
}
