using BuildingBlocks.Messaging.Outbox;
using BuildingBlocks.Messaging.Outbox.EF;
using BuildingBlocks.Persistence.EfCore.Postgres;
using BuildingBlocks.Scheduling.Internal;

namespace ECommerce.Services.Catalogs.Shared.Data;

public class CatalogDbContextDesignFactory : DbContextDesignFactoryBase<CatalogDbContext>
{
    public CatalogDbContextDesignFactory() : base("CatalogServiceConnection")
    {
    }
}

public class OutboxDbContextDesignFactory : DbContextDesignFactoryBase<OutboxDataContext>
{
    public OutboxDbContextDesignFactory() : base("CatalogServiceConnection")
    {
    }
}

public class InternalMessageDbContextDesignFactory : DbContextDesignFactoryBase<InternalMessageDbContext>
{
    public InternalMessageDbContextDesignFactory() : base("CatalogServiceConnection")
    {
    }
}
