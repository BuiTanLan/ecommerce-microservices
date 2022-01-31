namespace ECommerce.Services.Customers.Customers.Dtos;

public record CustomerDto
{
    public Guid IdentityId { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Nationality { get; set; }
    public string? BirthDate { get; set; }
    public string? PhoneNumber { get; set; } = null!;
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
}
