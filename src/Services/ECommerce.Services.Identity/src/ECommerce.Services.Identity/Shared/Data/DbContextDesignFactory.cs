using BuildingBlocks.EFCore;
using BuildingBlocks.Messaging.Outbox;

namespace ECommerce.Services.Identity.Shared.Data;

public class DbContextDesignFactory : DbContextDesignFactoryBase<IdentityContext>
{
    public DbContextDesignFactory() : base("IdentityServiceConnection")
    {
    }
}

public class OutboxDbContextDesignFactory : DbContextDesignFactoryBase<OutboxDataContext>
{
    public OutboxDbContextDesignFactory() : base("IdentityServiceConnection")
    {
    }
}
