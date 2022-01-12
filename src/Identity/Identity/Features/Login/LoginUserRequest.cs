namespace Identity.Features.Login;

public class LoginUserRequest
{
    public string UserNameOrEmail { get; set; }
    public string Password { get; set; }
    public bool Remember { get; set; }
}
