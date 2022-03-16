using MicroBootstrap.Persistence.EfCore.Postgres;
using MicroBootstrap.Scheduling.Internal;

namespace ECommerce.Services.Identity.Shared.Data;

public class DbContextDesignFactory : DbContextDesignFactoryBase<IdentityContext>
{
    public DbContextDesignFactory() : base("IdentityServiceConnection")
    {
    }
}