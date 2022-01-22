namespace Catalog.Products.Features.CreateProduct.Command;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(x => x.Price)
            .NotEmpty()
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Stock)
            .NotEmpty()
            .GreaterThan(0).WithMessage("Stock must be greater than 0");

        RuleFor(x => x.MaxStockThreshold)
            .NotEmpty()
            .GreaterThan(0).WithMessage("MaxStockThreshold must be greater than 0");

        RuleFor(x => x.RestockThreshold)
            .NotEmpty()
            .GreaterThan(0).WithMessage("RestockThreshold must be greater than 0");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .GreaterThan(0).WithMessage("CategoryId must be greater than 0");

        RuleFor(x => x.SupplierId)
            .NotEmpty()
            .GreaterThan(0).WithMessage("SupplierId must be greater than 0");

        RuleFor(x => x.BrandId)
            .NotEmpty()
            .GreaterThan(0).WithMessage("BrandId must be greater than 0");
    }
}
