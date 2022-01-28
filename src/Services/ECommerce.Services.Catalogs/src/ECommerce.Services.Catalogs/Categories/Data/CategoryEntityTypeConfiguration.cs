using ECommerce.Services.Catalogs.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Services.Catalogs.Categories.Data;

public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories", CatalogDbContext.DefaultSchema);
        builder.HasKey(c => c.Id);
        builder.HasIndex(x => x.Id).IsUnique();
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => id)
            .ValueGeneratedNever();

        builder.Ignore(c => c.DomainEvents);

        builder.Property(x => x.Created).HasDefaultValueSql(Constants.DateAlgorithm);
        builder.Property(x => x.Name).HasColumnType(Constants.NormalText).IsRequired();
    }
}
