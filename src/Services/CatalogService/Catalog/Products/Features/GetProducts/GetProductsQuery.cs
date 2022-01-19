using AutoMapper;
using BuildingBlocks.CQRS;
using BuildingBlocks.CQRS.Query;
using Catalog.Core.Contracts;
using Catalog.Products.Dtos;

namespace Catalog.Products.Features.GetProducts;

public record GetProductsQuery : IListQuery<GetProductsQueryResult>
{
    public List<string>? Includes { get; init; } = new(new[] { nameof(Product.Supplier), nameof(Product.Category) });
    public List<FilterModel>? Filters { get; init; } = new();
    public List<string>? Sorts { get; init; } = new();
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, GetProductsQueryResult>
{
    private readonly ICatalogDbContext _catalogDbContext;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(ICatalogDbContext catalogDbContext, IMapper mapper)
    {
        _catalogDbContext = catalogDbContext;
        _mapper = mapper;
    }

    public async Task<GetProductsQueryResult> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _catalogDbContext.Products
            .ApplyIncludeList(request.Includes)
            .ApplyFilterList(request.Filters)
            .PaginateAsync<Product, ProductDto>(_mapper.ConfigurationProvider, request.Page, request.PageSize);

        return new GetProductsQueryResult(products);
    }
}
