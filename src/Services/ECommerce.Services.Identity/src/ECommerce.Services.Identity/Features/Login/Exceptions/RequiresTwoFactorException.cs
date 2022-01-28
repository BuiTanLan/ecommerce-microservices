using BuildingBlocks.Exception;

namespace ECommerce.Services.Identity.Features.Login.Exceptions;

public class RequiresTwoFactorException : BadRequestException
{
    public RequiresTwoFactorException(string message) : base(message)
    {
    }
}
