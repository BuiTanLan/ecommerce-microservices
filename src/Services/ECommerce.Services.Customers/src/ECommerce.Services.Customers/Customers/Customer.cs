using BuildingBlocks.Core.Domain.Model;
using ECommerce.Services.Customers.Customers.ValueObjects;

namespace ECommerce.Services.Customers.Customers;

public class Customer : AggregateRoot<CustomerId>
{
    public static Customer Create(Guid id, Email email, DateTime createdAt)
    {
        var customer = new Customer() { }
        // Id = id;
        // Email = email ?? throw new InvalidCustomerEmailException(Id);
        // IsActive = true;
        // CreatedAt = createdAt;
    }

    public void Complete(
        FirstName name,
        LastName fullName,
        Address address,
        Nationality nationality,
        DateTime completedAt)
    {
        if (!IsActive)
        {
            throw new CustomerNotActiveException(Id);
        }

        if (CompletedAt.HasValue)
        {
            throw new CannotCompleteCustomerException(Id);
        }

        Name = name ?? throw new InvalidCustomerNameException(Id);
        FullName = fullName;
        Address = address;
        Nationality = nationality;
        CompletedAt = completedAt;
    }

    public Guid IdentityId { get; private set; }
    public Email Email { get; private set; }
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Address Address { get; private set; }
    public Nationality Nationality { get; private set; }
    public BirthDate BirthDate { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
}
