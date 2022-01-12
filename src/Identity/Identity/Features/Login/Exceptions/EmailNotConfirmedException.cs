using BuildingBlocks.Exception;

namespace Identity.Features.Login.Exceptions;

public class EmailNotConfirmedException : BadRequestException
{
    public string Email { get; }

    public EmailNotConfirmedException(string email) : base($"Email not confirmed for email address `{email}`")
    {
        Email = email;
    }
}
