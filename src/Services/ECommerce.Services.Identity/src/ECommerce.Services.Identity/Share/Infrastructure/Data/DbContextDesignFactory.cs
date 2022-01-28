using BuildingBlocks.EFCore;

namespace ECommerce.Services.Identity.Share.Infrastructure.Data;

public class DbContextDesignFactory : DbContextDesignFactoryBase<IdentityContext>
{
    public DbContextDesignFactory() : base("IdentityServiceConnection")
    {
    }
}
