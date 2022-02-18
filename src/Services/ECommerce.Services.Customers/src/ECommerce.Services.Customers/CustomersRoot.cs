using System.Runtime.CompilerServices;
using NullGuard;

[assembly: InternalsVisibleTo("ECommerce.Services.Customers.IntegrationTests")]
[assembly: NullGuard(ValidationFlags.All)]

namespace ECommerce.Services.Customers;

public class CustomersRoot
{
}
