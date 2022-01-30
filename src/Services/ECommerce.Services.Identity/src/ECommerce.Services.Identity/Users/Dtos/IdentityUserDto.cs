using ECommerce.Services.Identity.Shared.Models;

namespace ECommerce.Services.Identity.Users.Dtos;

public class IdentityUserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? LastLoggedInAt { get; set; }
    public IEnumerable<string> RefreshTokens { get; set; }
    public IEnumerable<string> Roles { get; set; }
    public UserState UserState { get; set; }
    public DateTime CreatedAt { get; set; }
}
