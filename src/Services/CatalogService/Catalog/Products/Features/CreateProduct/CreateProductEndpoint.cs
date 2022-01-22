using System.ComponentModel;
using System.Globalization;
using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.CQRS.Command;
using Catalog.Products.Features.CreateProduct.Command;
using Catalog.Products.Features.CreateProduct.Requests;

namespace Catalog.Products.Features.CreateProduct;

// POST api/v1/catalog/products
public static class CreateProductEndpoint
{
    internal static IEndpointRouteBuilder MapCreateProductsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{CatalogConfiguration.CatalogModulePrefixUri}{Configs.ProductsPrefixUri}", CreateProducts)
            .WithTags(Configs.Tag)
            // .RequireAuthorization()
            .Produces<CreateProductCommandResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateProduct")
            .WithDisplayName("Create a new product.");

        return endpoints;
    }

    private static async Task<IResult> CreateProducts(
        CreateProductRequest request,
        ICommandProcessor commandProcessor,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        var command = mapper.Map<CreateProductCommand>(request);
        var result = await commandProcessor.SendAsync(command, cancellationToken);

        return Results.CreatedAtRoute("GetProductById", new { id = result.Product.Id }, result);
    }
}
