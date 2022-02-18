using System.Runtime.CompilerServices;
using NullGuard;

[assembly: InternalsVisibleTo("ECommerce.Services.Catalogs.IntegrationTests")]
[assembly: NullGuard(ValidationFlags.All)]

namespace ECommerce.Services.Catalogs;

public class CatalogRoot
{
}
