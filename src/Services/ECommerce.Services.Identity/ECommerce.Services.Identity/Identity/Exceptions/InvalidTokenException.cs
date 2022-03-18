using MicroBootstrap.Core.Exception.Types;


namespace ECommerce.Services.Identity.Identity.Exceptions;

public class InvalidTokenException : BadRequestException
{
    public InvalidTokenException() : base("access_token is invalid!")
    {
    }
}
