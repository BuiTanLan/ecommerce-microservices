using BuildingBlocks.Core.Domain.ValueObjects;
using ECommerce.Services.Customers.Customers.Models;
using ECommerce.Services.Customers.Customers.ValueObjects;
using ECommerce.Services.Customers.Shared.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Services.Customers.Customers.Data;

public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers", CustomersDbContext.DefaultSchema);

        builder.Ignore(c => c.DomainEvents);

        builder.HasKey(c => c.Id);
        builder.HasIndex(x => x.Id).IsUnique();

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, id => new CustomerId(id))
            .ValueGeneratedNever();

        builder.Property(x => x.CustomerState)
            .HasDefaultValue(CustomerState.None)
            .HasConversion(
                x => x.ToString(),
                x => (CustomerState)Enum.Parse(typeof(CustomerState), x));

        builder.HasIndex(x => x.IdentityId).IsUnique();

        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.Email).IsRequired()
            .HasMaxLength(Constants.Lenght.Medium)
            .HasConversion(email => email.Value, email => Email.Create(email)); // converting mandatory value objects

        builder.HasIndex(x => x.PhoneNumber).IsUnique();
        builder.Property(x => x.PhoneNumber)
            .IsRequired(false)
            .HasMaxLength(Constants.Lenght.Tiny)
            .HasConversion(x => (string?)x, x => (PhoneNumber?)x);

        builder.OwnsOne(m => m.Name, a =>
        {
            a.Property(p => p.FirstName)
                .HasMaxLength(Constants.Lenght.Medium);

            a.Property(p => p.LastName)
                .HasMaxLength(Constants.Lenght.Medium);

            a.Ignore(p => p.FullName);
        });

        builder.OwnsOne(m => m.Address, a =>
        {
            a.Property(p => p.City)
                .HasMaxLength(Constants.Lenght.Short);

            a.Property(p => p.Country)
                .HasMaxLength(Constants.Lenght.Medium);

            a.Property(p => p.Detail)
                .HasMaxLength(Constants.Lenght.Medium);
        });

        builder.Property(x => x.Nationality)
            .IsRequired(false)
            .HasMaxLength(Constants.Lenght.Short)
            .HasConversion(
                nationality => (string?)nationality,
                nationality => (Nationality?)nationality); // converting optional value objects

        builder.Property(x => x.BirthDate)
            .IsRequired(false)
            .HasConversion(x => (DateTime?)x, x => (BirthDate?)x);

        builder.Property(x => x.Created).HasDefaultValueSql(Constants.DateAlgorithm);
    }
}
