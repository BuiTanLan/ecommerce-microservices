using Catalog.Products.Data;
using Catalog.Products.Features.CreatingProduct;
using Catalog.Products.Features.GettingProducts;
using Catalog.Products.Features.GettingProductsView;

namespace Catalog.Products;

internal static class ProductsConfigs
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
