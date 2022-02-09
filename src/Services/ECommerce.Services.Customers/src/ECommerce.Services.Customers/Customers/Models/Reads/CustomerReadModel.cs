namespace ECommerce.Services.Customers.Customers.Models.Reads;

public class CustomerReadModel
{
    public long Id { get; init; }
    public Guid IdentityId { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string FullName { get; init; } = null!;
    public string? Country { get; init; }
    public string? City { get; init; }
    public string? DetailAddress { get; init; }
    public string? Nationality { get; init; }
    public DateTime? BirthDate { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; }
    public DateTime? CompletedAt { get; init; }
    public DateTime? VerifiedAt { get; init; }
    public CustomerState CustomerState { get; init; } = CustomerState.None;
}
