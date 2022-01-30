using ECommerce.Services.Identity.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.Identity.Shared.Data;

public class IdentityContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public IdentityContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
