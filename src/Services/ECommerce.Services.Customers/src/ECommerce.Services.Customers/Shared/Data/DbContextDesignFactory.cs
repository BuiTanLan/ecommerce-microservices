using MicroBootstrap.Persistence.EfCore.Postgres;

namespace ECommerce.Services.Customers.Shared.Data;

public class CatalogDbContextDesignFactory : DbContextDesignFactoryBase<CustomersDbContext>
{
    public CatalogDbContextDesignFactory() : base("CustomersServiceConnection")
    {
    }
}
