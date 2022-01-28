using BuildingBlocks.EFCore;
using ECommerce.Services.Identity.Share.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Services.Identity.Share.Infrastructure.Data.EntityConfigurations;

internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.UserName).HasMaxLength(50).IsRequired();
        builder.Property(x => x.NormalizedUserName).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(50).IsRequired();
        builder.Property(x => x.NormalizedEmail).HasMaxLength(50).IsRequired();
        builder.Property(x => x.PhoneNumber).HasMaxLength(15).IsRequired(false);

        builder.Property(x => x.CreatedAt).HasDefaultValueSql(Constants.DateAlgorithm);

        builder.Property(x => x.UserState)
            .HasDefaultValue(UserState.Active)
            .HasConversion(x => x.ToString(), x => (UserState)Enum.Parse(typeof(UserState), x));

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.NormalizedEmail).IsUnique();
    }
}
