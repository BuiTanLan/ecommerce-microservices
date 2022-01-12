
using BuildingBlocks.Exception;

namespace Identity.Features.RegisterNewUser;

public class RegisterIdentityUserException : AppException
{
    public RegisterIdentityUserException(string error) : base(error)
    {
    }
}
