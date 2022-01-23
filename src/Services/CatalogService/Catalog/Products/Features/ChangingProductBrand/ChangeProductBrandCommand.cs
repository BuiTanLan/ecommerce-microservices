using BuildingBlocks.CQRS.Command;

namespace Catalog.Products.Features.ChangingProductBrand;

internal record ChangeProductBrandCommand : ICommand<ChangeProductBrandCommandResult>;

internal record ChangeProductBrandCommandResult;

internal class ChangeProductBrandCommandHandler :
    ICommandHandler<ChangeProductBrandCommand, ChangeProductBrandCommandResult>
{
    public Task<ChangeProductBrandCommandResult> Handle(
        ChangeProductBrandCommand request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
