using BuildingBlocks.Exception;

namespace Identity.Features.Login.Exceptions;

public class PasswordIsInvalidException : BadRequestException
{
    public PasswordIsInvalidException() : base("password is invalid!")
    {
    }
}
