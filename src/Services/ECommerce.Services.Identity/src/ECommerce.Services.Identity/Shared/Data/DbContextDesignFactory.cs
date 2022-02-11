using BuildingBlocks.EFCore;
using BuildingBlocks.Messaging.Outbox;
using BuildingBlocks.Messaging.Outbox.EF;
using BuildingBlocks.Scheduling.Internal;

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

public class InternalMessageDbContextDesignFactory : DbContextDesignFactoryBase<InternalMessageDbContext>
{
    public InternalMessageDbContextDesignFactory() : base("IdentityServiceConnection")
    {
    }
}
