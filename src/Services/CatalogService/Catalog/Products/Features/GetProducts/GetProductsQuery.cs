using AutoMapper;
using BuildingBlocks.CQRS;
using BuildingBlocks.CQRS.Query;
using Catalog.Core.Contracts;
using Catalog.Products.Dtos;
using Catalog.Products.Models;

namespace Catalog.Products.Features.GetProducts;

public record GetProductsQuery : IListQuery<GetProductsQueryResult>
{
    public IList<string>? Includes { get; init; }
    public IList<FilterModel>? Filters { get; init; }
    public IList<string>? Sorts { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
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
            .AsNoTracking()
            .PaginateAsync<Product, ProductDto>(_mapper.ConfigurationProvider, request.Page, request.PageSize);

        return new GetProductsQueryResult(products);
    }
}
