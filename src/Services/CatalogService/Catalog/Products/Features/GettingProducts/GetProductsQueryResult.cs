using BuildingBlocks.CQRS;
using Catalog.Products.Dtos;

namespace Catalog.Products.Features.GettingProducts;

public record GetProductsQueryResult(ListResultModel<ProductDto> Products);

