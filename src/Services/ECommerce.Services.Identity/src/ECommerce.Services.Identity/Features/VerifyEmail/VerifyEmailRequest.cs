namespace ECommerce.Services.Identity.Features.VerifyEmail;

public class VerifyEmailRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
}
