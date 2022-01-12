using BuildingBlocks.Exception;

namespace Identity.Features.Login.Exceptions;

public class LoginFailedException : AppException
{
    public string UserNameOrEmail { get; }
    public LoginFailedException(string userNameOrEmail) : base($"Login failed for username: {userNameOrEmail}")
    {
        UserNameOrEmail = userNameOrEmail;
    }
}
