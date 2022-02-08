using BuildingBlocks.Core.Domain.ValueObjects;
using ECommerce.Services.Customers.RestockSubscriptions.Models;
using ECommerce.Services.Customers.Shared.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Services.Customers.RestockSubscriptions.Data;

public class RestockSubscriptionEntityConfiguration : IEntityTypeConfiguration<RestockSubscription>
{
    public void Configure(EntityTypeBuilder<RestockSubscription> builder)
    {
        builder.ToTable("restock_subscriptions", CustomersDbContext.DefaultSchema);

        builder.HasKey(c => c.Id);
        builder.HasIndex(x => x.Id).IsUnique();
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, id => id);

        builder.Ignore(c => c.DomainEvents);

        builder.Property(x => x.Processed).HasDefaultValue(false);

        builder.Property(c => c.CustomerId)
            .HasConversion(id => id.Value, id => id);

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId);

        builder.OwnsOne(x => x.ProductInformation, p =>
        {
            p.Property(x => x.Id)
                .HasConversion(id => id.Value, id => id);

            p.Property(x => x.Name)
                .HasMaxLength(Constants.Lenght.Normal);
        });

        builder.Property(x => x.Email)
            .HasConversion(email => email.Value, email => Email.Create(email));

        builder.Property(x => x.Created).HasDefaultValueSql(Constants.DateAlgorithm);
    }
}
