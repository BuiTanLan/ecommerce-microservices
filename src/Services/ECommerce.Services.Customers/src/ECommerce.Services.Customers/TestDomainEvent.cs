using BuildingBlocks.Abstractions.Domain.Events.Internal;

namespace ECommerce.Services.Customers;

public record TestDomainEvent(string Data) : DomainEvent
{
}
