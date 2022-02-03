using BuildingBlocks.Messaging.Outbox;
using BuildingBlocks.Messaging.Outbox.EF;

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
