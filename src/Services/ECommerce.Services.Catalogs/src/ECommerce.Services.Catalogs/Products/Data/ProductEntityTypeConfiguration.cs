using ECommerce.Services.Catalogs.Products.Models;
using ECommerce.Services.Catalogs.Products.Models.ValueObjects;
using ECommerce.Services.Catalogs.Shared.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Services.Catalogs.Products.Data;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", CatalogDbContext.DefaultSchema);

        builder.HasKey(c => c.Id);
        builder.HasIndex(x => x.Id).IsUnique();

        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => id)
            .ValueGeneratedNever();

        builder.Ignore(c => c.DomainEvents);

        builder.Property(x => x.Name)
            .HasColumnType(Constants.ColumnTypes.NormalText)
            .HasConversion(name => name.Value, name => new Name(name))
            .IsRequired();

        builder.Property(ci => ci.Price)
            .HasColumnType(Constants.ColumnTypes.PriceDecimal)
            .HasConversion(price => price.Value, price => price)
            .IsRequired();

        builder.Property(ci => ci.Size)
            .HasConversion(size => size.Value, size => new Size(size))
            .IsRequired();

        builder.Property(x => x.ProductStatus)
            .HasDefaultValue(ProductStatus.Available)
            .HasMaxLength(Constants.Lenght.Short)
            .HasConversion(
                x => x.ToString(),
                x => (ProductStatus)Enum.Parse(typeof(ProductStatus), x));

        builder.Property(x => x.Color)
            .HasDefaultValue(ProductColor.Black)
            .HasMaxLength(Constants.Lenght.Short)
            .HasConversion(
                x => x.ToString(),
                x => (ProductColor)Enum.Parse(typeof(ProductColor), x));

        builder.OwnsOne(c => c.Dimensions, cm =>
        {
            cm.Property(c => c.Height);
            cm.Property(c => c.Width);
            cm.Property(c => c.Depth);
        });

        builder.OwnsOne(c => c.Stock, cm =>
        {
            cm.Property(c => c.Available);
            cm.Property(c => c.RestockThreshold);
            cm.Property(c => c.MaxStockThreshold);
        });

        builder.HasOne(c => c.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);

        builder.HasOne(c => c.Brand)
            .WithMany()
            .HasForeignKey(x => x.BrandId);

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
