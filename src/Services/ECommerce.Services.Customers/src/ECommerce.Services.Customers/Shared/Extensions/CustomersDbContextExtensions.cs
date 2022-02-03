using ECommerce.Services.Customers.Customers.Models;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Customers.Shared.Extensions;

public static class CustomersDbContextExtensions
{
    public static ValueTask<Customer?> FindCustomerByIdAsync(
        this CustomersDbContext context,
        long id,
        CancellationToken cancellationToken = default)
    {
        return context.Customers.FindAsync(id, cancellationToken);
    }

    public static Task<bool> ExistsCustomerByIdAsync(
        this CustomersDbContext context,
        long id,
        CancellationToken cancellationToken = default)
    {
        return context.Customers.AnyAsync(x => x.Id == id, cancellationToken);
    }
}
