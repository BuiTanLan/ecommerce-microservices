using BuildingBlocks.Core.Domain.Events.Internal;

namespace ECommerce.Services.Catalogs.Products.Events.Domain;

public record RestockThresholdChanged(int RestockThreshold) : DomainEvent;
