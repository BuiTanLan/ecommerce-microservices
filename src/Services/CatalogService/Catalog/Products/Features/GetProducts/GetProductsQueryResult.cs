using BuildingBlocks.CQRS;
using Catalog.Products.Dtos;

namespace Catalog.Products.Features.GetProducts;

public record GetProductsQueryResult(ListResultModel<ProductDto> Products);

