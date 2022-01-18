using BuildingBlocks.EFCore;

namespace Catalog.Infrastructure.Data;

public class CatalogDataSeeder:IDataSeeder
{
    public Task SeedAllAsync()
    {
        return Task.CompletedTask;
    }
}
