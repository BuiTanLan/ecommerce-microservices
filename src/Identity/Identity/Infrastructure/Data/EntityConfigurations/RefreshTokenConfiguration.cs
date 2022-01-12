using Identity.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Data.EntityConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(x => new { x.Token, x.UserId });

        builder.HasOne(rt => rt.ApplicationUser)
            .WithMany(au => au.RefreshTokens);

        builder.Property(rt => rt.Token).HasMaxLength(100);
        builder.Property(rt => rt.CreatedAt);
        builder.Ignore(rt => rt.IsActive);
        builder.Ignore(rt => rt.IsExpired);
    }
}
