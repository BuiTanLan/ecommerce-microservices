using AutoMapper;
using BuildingBlocks.CQRS.Query;
using Dapper;

namespace Catalog.Products.Features.GetProductsView;

public class GetProductsViewQuery : IQuery<GetProductsViewQueryResult>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

internal class GetProductsViewQueryHandler : IRequestHandler<GetProductsViewQuery, GetProductsViewQueryResult>
{
    private readonly IDbFacadeResolver _facadeResolver;
    private readonly IMapper _mapper;

    public GetProductsViewQueryHandler(IDbFacadeResolver facadeResolver, IMapper mapper)
    {
        _facadeResolver = facadeResolver;
        _mapper = mapper;
    }

    public async Task<GetProductsViewQueryResult> Handle(
        GetProductsViewQuery request,
        CancellationToken cancellationToken)
    {
        await using var conn = _facadeResolver.Database.GetDbConnection();
        await conn.OpenAsync(cancellationToken);
        var results = await conn.QueryAsync<ProductView>(
            @"SELECT product_id ""Id"", product_name ""Name"", category_name CategoryName, supplier_name SupplierName, count(*) OVER() AS ItemCount
                    FROM catalog.product_views LIMIT @PageSize OFFSET ((@Page - 1) * @PageSize)",
            new { request.PageSize, request.Page }
        );

        var productViewDtos = _mapper.Map<IEnumerable<ProductViewDto>>(results);

        return new GetProductsViewQueryResult(productViewDtos);
    }
}
