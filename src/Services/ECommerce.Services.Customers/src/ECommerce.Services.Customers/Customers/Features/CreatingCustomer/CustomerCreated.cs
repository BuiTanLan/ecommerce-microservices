using BuildingBlocks.Core.Domain.Events.Internal;
using ECommerce.Services.Customers.Customers.Models;

namespace ECommerce.Services.Customers.Customers.Features.CreatingCustomer;

public record CustomerCreated(Customer Customer) : DomainEvent;
