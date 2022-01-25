using BuildingBlocks.CQRS.Command;

namespace Catalog.Products.Features.ChangingProductBrand;

internal record ChangeProductBrand : ICommand<ChangeProductBrandResult>;

internal record ChangeProductBrandResult;

internal class ChangeProductBrandHandler :
    ICommandHandler<ChangeProductBrand, ChangeProductBrandResult>
{
    public Task<ChangeProductBrandResult> Handle(
        ChangeProductBrand command,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
