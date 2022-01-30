using BuildingBlocks.Exception;
using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Identity.Identity.Exceptions;

public class RequiresTwoFactorException : BadRequestException
{
    public RequiresTwoFactorException(string message) : base(message)
    {
    }
}
