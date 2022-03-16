using AutoMapper;
using ECommerce.Services.Catalogs.Products.Dtos;
using ECommerce.Services.Catalogs.Products.Models;
using ECommerce.Services.Catalogs.Shared.Contracts;
using MicroBootstrap.Abstractions.CQRS.Query;
using MicroBootstrap.Core.Persistence.EfCore;
using MicroBootstrap.Core.Types;
using MicroBootstrap.CQRS.Query;

namespace ECommerce.Services.Catalogs.Products.Features.GettingProducts;

public record GetProducts : ListQuery<GetProductsResult>;

public class GetProductsValidator : AbstractValidator<GetProducts>
{
    public GetProductsValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page should at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize should at least greater than or equal to 1.");
    }
}

public class GetProductsHandler : IQueryHandler<GetProducts, GetProductsResult>
{
    private readonly ICatalogDbContext _catalogDbContext;
    private readonly IMapper _mapper;

    public GetProductsHandler(ICatalogDbContext catalogDbContext, IMapper mapper)
    {
        _catalogDbContext = catalogDbContext;
        _mapper = mapper;
    }

    public async Task<GetProductsResult> Handle(GetProducts request, CancellationToken cancellationToken)
    {
        var products = await _catalogDbContext.Products
            .OrderByDescending(x => x.Created)
            .ApplyIncludeList(request.Includes)
            .ApplyFilterList(request.Filters)
            .AsNoTracking()
            .PaginateAsync<Product, ProductDto>(_mapper.ConfigurationProvider, request.Page, request.PageSize, cancellationToken: cancellationToken);

        return new GetProductsResult(products);
    }
}

public record GetProductsResult(ListResultModel<ProductDto> Products);
