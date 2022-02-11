using BuildingBlocks.Messaging.Outbox.EF;
using BuildingBlocks.Scheduling.Internal;

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

public class InternalMessageDbContextDesignFactory : DbContextDesignFactoryBase<InternalMessageDbContext>
{
    public InternalMessageDbContextDesignFactory() : base("CustomersServiceConnection")
    {
    }
}
