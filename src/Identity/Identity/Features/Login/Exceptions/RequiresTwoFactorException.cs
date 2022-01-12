using BuildingBlocks.Exception;

namespace Identity.Features.Login.Exceptions;

public class RequiresTwoFactorException : BadRequestException
{
    public RequiresTwoFactorException(string message) : base(message)
    {
    }
}
