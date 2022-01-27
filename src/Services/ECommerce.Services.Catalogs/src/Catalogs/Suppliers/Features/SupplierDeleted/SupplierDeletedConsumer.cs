using BuildingBlocks.Core.Domain.Events;
using Catalogs.Suppliers.Features.SupplierDeleted.External;

namespace Catalogs.Suppliers.Features.SupplierDeleted;

public class SupplierDeletedConsumer : IEventHandler<SupplierDeletedIntegrationEvent>
{
    public Task Handle(SupplierDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
