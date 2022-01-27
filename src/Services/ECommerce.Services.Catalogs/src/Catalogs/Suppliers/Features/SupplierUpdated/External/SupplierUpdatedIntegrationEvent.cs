using BuildingBlocks.Core.Domain.Events.External;

namespace Catalogs.Suppliers.Features.SupplierUpdated.External;

public record SupplierUpdatedIntegrationEvent(long Id, string Name) : IntegrationEvent;
