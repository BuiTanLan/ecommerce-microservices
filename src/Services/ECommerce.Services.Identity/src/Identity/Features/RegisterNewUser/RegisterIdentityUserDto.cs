namespace Identity.Features.RegisterNewUser;

public class RegisterIdentityUserDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public bool IsAdmin { get; init; }
}
