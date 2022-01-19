using Catalog.Suppliers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.EntityConfigurations;

public class SupplierEntityTypeConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers", CatalogDbContext.DefaultSchema);
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();
    }
}
