using BuildingBlocks.Core.Domain.Events.External;

namespace Catalogs.Suppliers.Features.SupplierDeleted.External;

public record SupplierDeletedIntegrationEvent(long Id) : IntegrationEvent;
