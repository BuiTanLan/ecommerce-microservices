using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.CQRS.Query;
using ECommerce.Services.Catalogs.Products.Dtos;
using ECommerce.Services.Catalogs.Shared.Core.Contracts;
using ECommerce.Services.Catalogs.Shared.Infrastructure.Extensions;

namespace ECommerce.Services.Catalogs.Products.Features.GettingProductById;

public record GetProductById(long Id) : IQuery<GetProductByIdResult>;

internal class GetProductByIdValidator : AbstractValidator<GetProductById>
{
    public GetProductByIdValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class GetProductByIdHandler : IQueryHandler<GetProductById, GetProductByIdResult>
{
    private readonly ICatalogDbContext _catalogDbContext;
    private readonly IMapper _mapper;

    public GetProductByIdHandler(ICatalogDbContext catalogDbContext, IMapper mapper)
    {
        _catalogDbContext = catalogDbContext;
        _mapper = mapper;
    }

    public async Task<GetProductByIdResult> Handle(GetProductById query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var product = await _catalogDbContext.FindProductAsync(query.Id, cancellationToken);
        Guard.Against.NullProduct(product, query.Id);

        var productsDto = _mapper.Map<ProductDto>(product);

        return new GetProductByIdResult(productsDto);
    }
}

public record GetProductByIdResult(ProductDto Product);
