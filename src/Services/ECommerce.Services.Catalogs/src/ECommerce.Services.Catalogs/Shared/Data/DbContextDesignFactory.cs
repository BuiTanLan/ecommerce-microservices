using BuildingBlocks.Messaging.Outbox;
using BuildingBlocks.Messaging.Outbox.EF;

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
