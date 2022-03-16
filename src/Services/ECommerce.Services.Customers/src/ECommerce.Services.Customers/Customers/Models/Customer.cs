using Ardalis.GuardClauses;
using ECommerce.Services.Customers.Customers.Exceptions.Domain;
using ECommerce.Services.Customers.Customers.Features.CompletingCustomer.Events.Domain;
using ECommerce.Services.Customers.Customers.Features.CreatingCustomer.Events.Domain;
using ECommerce.Services.Customers.Customers.Features.LockingCustomer.Events.Domain;
using ECommerce.Services.Customers.Customers.Features.UnlockingCustomer.Events.Domain;
using ECommerce.Services.Customers.Customers.Features.VerifyingCustomer.Events.Domain;
using ECommerce.Services.Customers.Customers.ValueObjects;
using MicroBootstrap.Core.Domain.Model;
using MicroBootstrap.Core.Domain.ValueObjects;
using MicroBootstrap.Core.Exception;

namespace ECommerce.Services.Customers.Customers.Models;

public class Customer : Aggregate<CustomerId>
{
    public Guid IdentityId { get; private set; }
    public Email Email { get; private set; } = null!;
    public CustomerName Name { get; private set; } = null!;
    public Address? Address { get; private set; }
    public Nationality? Nationality { get; private set; }
    public BirthDate? BirthDate { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public CustomerState CustomerState { get; private set; } = CustomerState.None;

    public static Customer Create(CustomerId id, Email email, CustomerName name, Guid identityId)
    {
        var customer = new Customer
        {
            Id = Guard.Against.Null(id, nameof(id)),
            Email = Guard.Against.Null(email, nameof(email)),
            Name = Guard.Against.Null(name, nameof(name)),
            IdentityId = Guard.Against.NullOrEmpty(identityId, nameof(IdentityId)),
            CustomerState = CustomerState.New,
            IsActive = true
        };

        customer.AddDomainEvent(new CustomerCreated(customer));

        return customer;
    }

    public void Complete(
        PhoneNumber phoneNumber,
        DateTime completedAt,
        Address? address = null,
        Nationality? nationality = null,
        BirthDate? birthDate = null)
    {
        if (!IsActive)
        {
            throw new CustomerNotActiveException(Id);
        }

        if (CompletedAt.HasValue)
        {
            throw new CustomerAlreadyCompletedException(Id);
        }

        Address = address;
        Nationality = nationality;
        BirthDate = birthDate;
        CompletedAt = Guard.Against.InvalidDate(completedAt);
        PhoneNumber = Guard.Against.Null(phoneNumber, nameof(phoneNumber))!;
        CustomerState = CustomerState.Completed;

        AddDomainEvent(new CustomerCompleted(this));
    }

    public void Verify(DateTime verifiedAt)
    {
        if (!IsActive)
        {
            throw new CustomerNotActiveException(Id);
        }

        if (!CompletedAt.HasValue || VerifiedAt.HasValue)
        {
            throw new CustomerAlreadyVerifiedException(Id);
        }

        VerifiedAt = verifiedAt;
        CustomerState = CustomerState.Verified;

        AddDomainEvent(new CustomerVerified(this));
    }

    public void Lock(string? notes = null)
    {
        IsActive = false;
        Notes = notes?.Trim();
        CustomerState = CustomerState.Locked;

        AddDomainEvent(new CustomerLocked(this));
    }

    public void Unlock(string? notes = null)
    {
        IsActive = true;
        Notes = notes?.Trim();
        CustomerState =
            VerifiedAt != null ? CustomerState.Verified :
            CompletedAt != null ? CustomerState.Completed :
            CustomerState.New;

        AddDomainEvent(new CustomerUnlocked(this));
    }
}
