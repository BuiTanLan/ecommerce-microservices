using ECommerce.Services.Identity.Share.Core.Models;

namespace ECommerce.Services.Identity.Features.RegisteringUser;

public class RegisterIdentityUserDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public UserState UserState { get; init; }
    public DateTime CreatedAt { get; init; }
    public IEnumerable<string> Roles { get; set; }
}
