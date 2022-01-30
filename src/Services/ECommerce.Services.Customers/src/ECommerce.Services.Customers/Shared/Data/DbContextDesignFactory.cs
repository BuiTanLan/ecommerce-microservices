using BuildingBlocks.Messaging.Outbox;

namespace ECommerce.Services.Customers.Shared.Data;

public class CatalogDbContextDesignFactory : DbContextDesignFactoryBase<CustomersDbContext>
{
    public CatalogDbContextDesignFactory() : base("CustomersServiceConnection")
    {
    }
}

public class OutboxDbContextDesignFactory : DbContextDesignFactoryBase<OutboxDataContext>
{
    public OutboxDbContextDesignFactory() : base("CustomersServiceConnection")
    {
    }
}
