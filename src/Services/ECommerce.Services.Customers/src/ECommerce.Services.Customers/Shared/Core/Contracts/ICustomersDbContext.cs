using ECommerce.Services.Customers.Customers;

namespace ECommerce.Services.Customers.Shared.Core.Contracts;

public interface ICustomersDbContext
{
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    public DbSet<Customer> Products { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
