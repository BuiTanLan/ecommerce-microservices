using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalog.Products.Events.Domain;

public record MaxThresholdChanged(int MaxThreshold) : DomainEvent;
