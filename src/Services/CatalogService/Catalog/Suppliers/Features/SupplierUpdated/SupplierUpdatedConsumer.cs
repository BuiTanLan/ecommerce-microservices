using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Domain.Events;
using Catalog.Suppliers.Features.SupplierUpdated.External;

namespace Catalog.Suppliers.Features.SupplierUpdated;

public class SupplierUpdatedConsumer : IEventHandler<SupplierUpdatedIntegrationEvent>
{
    public Task Handle(SupplierUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
