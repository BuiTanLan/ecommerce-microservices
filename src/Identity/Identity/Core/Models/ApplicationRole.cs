using Microsoft.AspNetCore.Identity;

namespace Identity.Core.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public static ApplicationRole User => new() { Name = nameof(User), NormalizedName = nameof(User).ToUpper() };
        public static ApplicationRole Admin => new() { Name = nameof(Admin), NormalizedName = nameof(Admin).ToUpper() };
    }
}
