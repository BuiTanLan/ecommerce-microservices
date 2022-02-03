using BuildingBlocks.Core.Domain.Events.Internal;
using ECommerce.Services.Customers.RestockSubscriptions.Models;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.CreatingRestockSubscription;

public record RestockSubscriptionCreated(RestockSubscription RestockSubscription) : DomainEvent;
