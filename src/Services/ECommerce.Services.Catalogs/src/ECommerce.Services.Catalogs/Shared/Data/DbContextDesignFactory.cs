using BuildingBlocks.Messaging.Outbox;

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
