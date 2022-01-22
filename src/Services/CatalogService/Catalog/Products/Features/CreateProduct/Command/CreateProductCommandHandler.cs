using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.IdsGenerator;
using Catalog.Core.Contracts;
using Catalog.Infrastructure.Extensions;
using Catalog.Products.Dtos;
using Catalog.Products.Models;

namespace Catalog.Products.Features.CreateProduct.Command;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, CreateProductCommandResult>
{
    private readonly IMapper _mapper;
    private readonly ICatalogDbContext _catalogDbContext;

    public CreateProductCommandHandler(ICatalogDbContext catalogDbContext, IMapper mapper)
    {
        _mapper = Guard.Against.Null(mapper, nameof(mapper));
        _catalogDbContext = Guard.Against.Null(catalogDbContext, nameof(catalogDbContext));
    }

    public async Task<CreateProductCommandResult> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var images = request.Images?.Select(x =>
            new ProductImage(SnowFlakIdGenerator.NewId(), x.ImageUrl, x.IsMain, request.Id)).ToList();

        var product = await Product.CreateAsync(
            request.Id,
            request.Name,
            request.Stock,
            request.RestockThreshold,
            request.MaxStockThreshold,
            request.Status,
            new Dimensions(request.Width, request.Height, request.Depth),
            request.Description,
            request.Price,
            request.CategoryId,
            request.SupplierId,
            request.BrandId,
            images);

        var category = await _catalogDbContext.FindCategoryAsync(request.CategoryId, cancellationToken);
        var brand = await _catalogDbContext.FindBrandAsync(request.BrandId, cancellationToken);
        var supplier = await _catalogDbContext.FindSupplierAsync(request.SupplierId, cancellationToken);

        product.ChangeCategory(category);
        product.ChangeSupplier(supplier);
        product.ChangeBrand(brand);

        var created = await _catalogDbContext.Products.AddAsync(product, cancellationToken: cancellationToken);
        var productDto = _mapper.Map<ProductDto>(created.Entity);

        return new CreateProductCommandResult(productDto);
    }
}
