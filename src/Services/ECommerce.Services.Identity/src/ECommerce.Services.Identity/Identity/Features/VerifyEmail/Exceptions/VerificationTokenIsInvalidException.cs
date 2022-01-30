using BuildingBlocks.Exception;
using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Identity.Identity.Features.VerifyEmail.Exceptions;

public class VerificationTokenIsInvalidException : BadRequestException
{
    public VerificationTokenIsInvalidException(string userId) : base(
        $"verification token is invalid for userId '{userId}'.")
    {
    }
}
