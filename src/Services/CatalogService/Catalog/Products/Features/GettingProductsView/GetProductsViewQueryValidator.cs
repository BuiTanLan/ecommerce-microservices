namespace Catalog.Products.Features.GettingProductsView;

public class GetProductsViewQueryValidator : AbstractValidator<GetProductsViewQuery>
{
    public GetProductsViewQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page should at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize should at least greater than or equal to 1.");
    }
}
