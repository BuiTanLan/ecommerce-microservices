using Catalog.Products.Data;
using Catalog.Products.Features.CreateProduct;
using Catalog.Products.Features.GetProducts;
using Catalog.Products.Features.GetProductsView;

namespace Catalog.Products;

internal static class Configs
{
    public const string Tag = "Product";
    public const string ProductsPrefixUri = "/products";

    internal static IServiceCollection AddProductsServices(this IServiceCollection services)
    {
        services.AddScoped<IDataSeeder, ProductDataSeeder>();

        return services;
    }

    internal static IEndpointRouteBuilder MapProductsEndpoints(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapCreateProductsEndpoint()
            .MapGetProductsEndpoint()
            .MapGetProductsViewEndpoint();
}
