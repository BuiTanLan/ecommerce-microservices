using BuildingBlocks.Core.Domain.Events;
using Catalogs.Products.Data;
using Catalogs.Products.Features.CreatingProduct;
using Catalogs.Products.Features.DebitingProductStock;
using Catalogs.Products.Features.GettingProductById;
using Catalogs.Products.Features.GettingProducts;
using Catalogs.Products.Features.GettingProductsView;
using Catalogs.Products.Features.ReplenishingProductStock;

namespace Catalogs.Products;

internal static class ProductsConfigs
{
    public const string Tag = "Product";
    public const string ProductsPrefixUri = "/products";

    internal static IServiceCollection AddProductsServices(this IServiceCollection services)
    {
        services.AddScoped<IDataSeeder, ProductDataSeeder>();

        // services.AddSingleton<IEventMapper<Product>, ProductEventMapper>();
        services.AddSingleton<IEventMapper, ProductEventMapper>();

        return services;
    }

    internal static IEndpointRouteBuilder MapProductsEndpoints(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapCreateProductsEndpoint()
            .MapGetProductsEndpoint()
            .MapDebitProductStockEndpoint()
            .MapReplenishProductStockEndpoint()
            .MapGetProductByIdEndpoint()
            .MapGetProductsViewEndpoint();
}
