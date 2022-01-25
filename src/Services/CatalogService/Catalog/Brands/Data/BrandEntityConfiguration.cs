using Catalog.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Brands.Data;

public class BrandEntityConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("brands", CatalogDbContext.DefaultSchema);
        builder.HasKey(c => c.Id);
        builder.HasIndex(x => x.Id).IsUnique();

        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name).HasColumnType(Constants.NormalText).IsRequired();
        builder.Property(x => x.Created).HasDefaultValueSql(Constants.DateAlgorithm);

        builder.Ignore(x => x.DomainEvents);
    }
}
