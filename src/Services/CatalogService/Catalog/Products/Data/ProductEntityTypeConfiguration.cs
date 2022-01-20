using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Products.Data;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", CatalogDbContext.DefaultSchema);

        builder.HasKey(c => c.Id);
        builder.HasIndex(x => x.Id).IsUnique();
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Ignore(c => c.DomainEvents);

        builder.Property(ci => ci.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ci => ci.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(c => c.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);

        builder.HasOne(c => c.Supplier)
            .WithMany()
            .HasForeignKey(x => x.SupplierId);

        builder.Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);
    }
}
