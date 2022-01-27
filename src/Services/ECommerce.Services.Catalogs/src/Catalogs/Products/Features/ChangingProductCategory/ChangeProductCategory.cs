using BuildingBlocks.CQRS.Command;

namespace Catalogs.Products.Features.ChangingProductCategory;

internal record ChangeProductCategory : ICommand<ChangeProductCategoryResult>;

internal record ChangeProductCategoryResult;

internal class ChangeProductCategoryHandler :
    ICommandHandler<ChangeProductCategory, ChangeProductCategoryResult>
{
    public Task<ChangeProductCategoryResult> Handle(
        ChangeProductCategory command,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
