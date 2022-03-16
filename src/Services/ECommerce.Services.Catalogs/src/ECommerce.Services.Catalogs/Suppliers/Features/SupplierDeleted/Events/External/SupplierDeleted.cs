using MicroBootstrap.Abstractions.Core.Domain.Events;
using MicroBootstrap.Core.Domain.Events.External;

namespace ECommerce.Services.Catalogs.Suppliers.Features.SupplierDeleted.Events.External;

public record SupplierDeleted(long Id) : IntegrationEvent;

public class SupplierDeletedConsumer : IEventHandler<SupplierDeleted>
{
    public Task Handle(SupplierDeleted notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
