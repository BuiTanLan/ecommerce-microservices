using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.IdsGenerator;
using BuildingBlocks.Messaging.Outbox;
using Catalog.Brands;
using Catalog.Categories;
using Catalog.Products.Core.Dtos;
using Catalog.Products.Core.Models;
using Catalog.Products.Core.Models.ValueObjects;
using Catalog.Products.Features.CreatingProduct.Requests;
using Catalog.Products.Models;
using Catalog.Shared.Core.Contracts;
using Catalog.Shared.Infrastructure.Extensions;
using Catalog.Suppliers;

namespace Catalog.Products.Features.CreatingProduct;

public record CreateProduct(
    string Name,
    decimal Price,
    int Stock,
    int RestockThreshold,
    int MaxStockThreshold,
    ProductStatus Status,
    int Width,
    int Height,
    int Depth,
    long CategoryId,
    long SupplierId,
    long BrandId,
    string? Description = null,
    IEnumerable<CreateProductImageRequest>? Images = null) : ICreateCommand<CreateProductResult>
{
    public long Id { get; } = SnowFlakIdGenerator.NewId();

    public class CreateProductValidator : AbstractValidator<CreateProduct>
    {
        public CreateProductValidator()
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
}

public class CreateProductHandler : ICommandHandler<CreateProduct, CreateProductResult>
{
    private readonly IMapper _mapper;
    private readonly ICatalogDbContext _catalogDbContext;

    public CreateProductHandler(ICatalogDbContext catalogDbContext, IMapper mapper)
    {
        _mapper = Guard.Against.Null(mapper, nameof(mapper));
        _catalogDbContext = Guard.Against.Null(catalogDbContext, nameof(catalogDbContext));
    }

    public async Task<CreateProductResult> Handle(
        CreateProduct command,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var images = command.Images?.Select(x =>
            new ProductImage(SnowFlakIdGenerator.NewId(), x.ImageUrl, x.IsMain, command.Id)).ToList();

        var category = await _catalogDbContext.FindCategoryAsync(command.CategoryId, cancellationToken);
        Guard.Against.CategoryNotFound(category, command.CategoryId);

        var brand = await _catalogDbContext.FindBrandAsync(command.BrandId, cancellationToken);
        Guard.Against.BrandNotFound(brand, command.BrandId);

        var supplier = await _catalogDbContext.FindSupplierAsync(command.SupplierId, cancellationToken);
        Guard.Against.SupplierNotFound(supplier, command.SupplierId);

        var product = await Product.CreateAsync(
            command.Id,
            command.Name,
            command.Stock,
            command.RestockThreshold,
            command.MaxStockThreshold,
            command.Status,
            new Dimensions(command.Width, command.Height, command.Depth),
            command.Description,
            command.Price,
            category,
            supplier,
            brand,
            images);

        var created = await _catalogDbContext.Products.AddAsync(product, cancellationToken: cancellationToken);
        var productDto = _mapper.Map<ProductDto>(created.Entity);

        return new CreateProductResult(productDto);
    }
}

public record CreateProductResult(ProductDto Product);
