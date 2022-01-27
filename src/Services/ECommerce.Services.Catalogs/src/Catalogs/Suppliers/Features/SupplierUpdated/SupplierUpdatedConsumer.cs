using BuildingBlocks.Core.Domain.Events;
using Catalogs.Suppliers.Features.SupplierUpdated.External;

namespace Catalogs.Suppliers.Features.SupplierUpdated;

public class SupplierUpdatedConsumer : IEventHandler<SupplierUpdatedIntegrationEvent>
{
    public Task Handle(SupplierUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
