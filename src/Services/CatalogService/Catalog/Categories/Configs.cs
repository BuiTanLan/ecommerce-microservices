using Catalog.Categories.Data;

namespace Catalog.Categories;

internal static class Configs
{
    internal static IServiceCollection AddCategories(this IServiceCollection services)
    {
        services.AddScoped<IDataSeeder, CategoryDataSeeder>();

        return services;
    }
}
