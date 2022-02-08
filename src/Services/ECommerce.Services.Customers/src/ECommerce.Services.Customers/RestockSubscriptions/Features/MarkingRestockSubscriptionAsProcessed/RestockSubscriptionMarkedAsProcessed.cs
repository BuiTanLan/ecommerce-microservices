using BuildingBlocks.Core.Domain.Events.Internal;
using ECommerce.Services.Customers.RestockSubscriptions.ValueObjects;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.MarkingRestockSubscriptionAsProcessed;

public record RestockSubscriptionMarkedAsProcessed(RestockSubscriptionId Id, DateTime ProcessedTime) : DomainEvent;
