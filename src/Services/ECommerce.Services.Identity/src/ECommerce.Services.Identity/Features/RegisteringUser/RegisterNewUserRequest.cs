using ECommerce.Services.Identity.Share.Core;

namespace ECommerce.Services.Identity.Features.RegisteringUser;

public class RegisterNewUserRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string> { Constants.Role.User };
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}
