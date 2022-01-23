using BuildingBlocks.CQRS.Query;

namespace Catalog.Products.Features.GettingProductById;

public record GetProductByIdQuery(long Id) : IQuery<GetProductByIdQueryResult>;
