using Catalog.Products.Models;
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

        builder.Property(x => x.Name).HasColumnType(Constants.NormalText).IsRequired();

        builder.Property(ci => ci.Price)
            .HasColumnType(Constants.PriceDecimal)
            .IsRequired();

        builder.Property(x => x.ProductStatus)
            .HasConversion(
                x => x.ToString(),
                x => (ProductStatus)Enum.Parse(typeof(ProductStatus), x));

        builder.OwnsOne(c => c.Dimensions, cm =>
        {
            cm.Property(c => c.Height);
            cm.Property(c => c.Width);
            cm.Property(c => c.Depth);
        });

        builder.HasOne(c => c.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);

        builder.HasOne(c => c.Supplier)
            .WithMany()
            .HasForeignKey(x => x.SupplierId);

        builder.HasMany(s => s.Images)
            .WithOne(s => s.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Created).HasDefaultValueSql(Constants.DateAlgorithm);
    }
}
