using BuildingBlocks.Messaging.Outbox;

namespace Catalogs.Shared.Infrastructure.Data;

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
