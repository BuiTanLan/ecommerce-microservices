namespace ECommerce.Services.Identity.Features.RefreshToken;

public class RefreshTokenRequest
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
