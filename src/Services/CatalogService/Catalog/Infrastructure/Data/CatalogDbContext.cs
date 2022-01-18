namespace Catalog.Infrastructure.Data;

public class CatalogDbContext : AppDbContextBase
{
    public const string DefaultSchema = "catalog";

    public CatalogDbContext(DbContextOptions options) : base(options)
    {
    }

    public CatalogDbContext(DbContextOptions options, IMediator mediator) : base(options, mediator)
    {
    }
}
