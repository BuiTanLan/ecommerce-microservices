using BuildingBlocks.Core.Domain.Events.Internal;
using ECommerce.Services.Customers.Customers.Models;

namespace ECommerce.Services.Customers.Customers.Features.CreatingCustomer.Events.Domain;

public record CustomerCreated(Customer Customer) : DomainEvent;
