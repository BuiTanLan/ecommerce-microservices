using BuildingBlocks.Exception;
using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Identity.Identity.Features.Login;

public class LoginFailedException : AppException
{
    public LoginFailedException(string userNameOrEmail) : base($"Login failed for username: {userNameOrEmail}")
    {
        UserNameOrEmail = userNameOrEmail;
    }

    public string UserNameOrEmail { get; }
}
