using BuildingBlocks.Core.Domain.Events.External;

namespace ECommerce.Services.Catalogs.Suppliers.Features.SupplierDeleted.External;

public record SupplierDeletedIntegrationEvent(long Id) : IntegrationEvent;
