namespace Identity.Core.Models;

public class UserOldPassword
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public string PasswordHash { get; init; }

    public DateTime SetAtUtc { get; init; }

    public ApplicationUser User { get; init; }
}
