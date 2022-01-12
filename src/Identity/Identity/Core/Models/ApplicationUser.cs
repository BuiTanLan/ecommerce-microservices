using Microsoft.AspNetCore.Identity;

namespace Identity.Core.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DialCode { get; set; }
    public bool IsDisabled { get; set; }
    public DateTime? LastLoggedInAtUtc { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    public string Address { get; set; }
    public bool IsAdmin { get; set; }
}
