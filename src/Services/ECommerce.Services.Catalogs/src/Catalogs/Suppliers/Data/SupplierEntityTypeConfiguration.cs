using Catalogs.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalogs.Suppliers.Data;

public class SupplierEntityTypeConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers", CatalogDbContext.DefaultSchema);
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => id)
            .ValueGeneratedNever();

        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Created).HasDefaultValueSql(Constants.DateAlgorithm);
        builder.Property(x => x.Name).HasColumnType(Constants.NormalText).IsRequired();
    }
}
