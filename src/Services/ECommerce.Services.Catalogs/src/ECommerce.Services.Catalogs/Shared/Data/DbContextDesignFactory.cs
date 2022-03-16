using MicroBootstrap.Persistence.EfCore.Postgres;
using MicroBootstrap.Scheduling.Internal;

namespace ECommerce.Services.Catalogs.Shared.Data;

public class CatalogDbContextDesignFactory : DbContextDesignFactoryBase<CatalogDbContext>
{
    public CatalogDbContextDesignFactory() : base("CatalogServiceConnection")
    {
    }
}