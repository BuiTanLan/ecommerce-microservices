using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalogs.Products.Events.Domain;

public record MaxThresholdChanged(int MaxThreshold) : DomainEvent;
