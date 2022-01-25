using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.IdsGenerator;
using BuildingBlocks.Messaging.Outbox;
using Catalog.Products.Dtos;
using Catalog.Products.Features.CreatingProduct.Requests;
using Catalog.Products.Models;
using Catalog.Products.Models.ValueObjects;
using Catalog.Shared.Core.Contracts;
using Catalog.Shared.Infrastructure.Extensions;

namespace Catalog.Products.Features.CreatingProduct;

public record CreateProductCommand(
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
    IEnumerable<CreateProductImageRequest>? Images = null) : ICreateCommand<CreateProductCommandResult>
{
    public long Id { get; } = SnowFlakIdGenerator.NewId();

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
}

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, CreateProductCommandResult>
{
    private readonly IOutboxService _outboxService;
    private readonly IMapper _mapper;
    private readonly ICatalogDbContext _catalogDbContext;

    public CreateProductCommandHandler(ICatalogDbContext catalogDbContext, IMapper mapper, IOutboxService outboxService)
    {
        _outboxService = Guard.Against.Null(outboxService, nameof(outboxService));
        _mapper = Guard.Against.Null(mapper, nameof(mapper));
        _catalogDbContext = Guard.Against.Null(catalogDbContext, nameof(catalogDbContext));
    }

    public async Task<CreateProductCommandResult> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var images = command.Images?.Select(x =>
            new ProductImage(SnowFlakIdGenerator.NewId(), x.ImageUrl, x.IsMain, command.Id)).ToList();

        var category = await _catalogDbContext.FindCategoryAsync(command.CategoryId, cancellationToken);
        var brand = await _catalogDbContext.FindBrandAsync(command.BrandId, cancellationToken);
        var supplier = await _catalogDbContext.FindSupplierAsync(command.SupplierId, cancellationToken);

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

        // await _outboxService.SaveAsync(
        //     new ProductCreatedIntegrationEvent(
        //         product.Id,
        //         product.Name,
        //         product.CategoryId,
        //         product.Category.Name,
        //         product.AvailableStock));

        return new CreateProductCommandResult(productDto);
    }
}

public record CreateProductCommandResult(ProductDto Product);
