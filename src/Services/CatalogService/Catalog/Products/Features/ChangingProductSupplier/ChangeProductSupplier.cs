using BuildingBlocks.CQRS.Command;

namespace Catalog.Products.Features.ChangingProductSupplier;

internal record ChangeProductSupplier : ICommand<ChangeProductSupplierResult>;

internal record ChangeProductSupplierResult;

internal class ChangeProductSupplierCommandHandler :
    ICommandHandler<ChangeProductSupplier, ChangeProductSupplierResult>
{
    public Task<ChangeProductSupplierResult> Handle(
        ChangeProductSupplier request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
