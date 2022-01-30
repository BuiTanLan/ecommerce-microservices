using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;
using BuildingBlocks.Core.ValueObjects;
using BuildingBlocks.Exception;
using ECommerce.Services.Customers.Customers.Events.Domain;
using ECommerce.Services.Customers.Customers.Exceptions;
using ECommerce.Services.Customers.Customers.Features.CompletingCustomer.Events.Domain;
using ECommerce.Services.Customers.Customers.Features.CreatingCustomer;
using ECommerce.Services.Customers.Customers.ValueObjects;

namespace ECommerce.Services.Customers.Customers.Models;

public class Customer : AggregateRoot<CustomerId>
{
    public Guid IdentityId { get; private set; }
    public Email Email { get; private set; } = null!;
    public Name Name { get; private set; } = null!;
    public Address? Address { get; private set; }
    public Nationality? Nationality { get; private set; }
    public BirthDate? BirthDate { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; } = null!;
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? VerifiedAt { get; private set; }

    public static Customer Create(CustomerId id, Email email, Name name, Guid identityId)
    {
        var customer = new Customer
        {
            Id = Guard.Against.Null(id, nameof(id)),
            Email = Guard.Against.Null(email, nameof(email)),
            Name = Guard.Against.Null(name, nameof(name)),
            IdentityId = Guard.Against.NullOrEmpty(identityId, nameof(IdentityId))
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
            throw new CannotCompleteCustomerException(Id);
        }

        Address = address;
        Nationality = nationality;
        BirthDate = birthDate;
        CompletedAt = Guard.Against.InvalidDate(completedAt);
        PhoneNumber = Guard.Against.Null(phoneNumber, nameof(phoneNumber))!;

        AddDomainEvent(new CustomerCompleted(
            Id,
            PhoneNumber!,
            CompletedAt.Value,
            Address?.Country,
            address?.City,
            Address?.Detail,
            Nationality));
    }

    public void Verify(DateTime verifiedAt)
    {
        if (!IsActive)
        {
            throw new CustomerNotActiveException(Id);
        }

        if (!CompletedAt.HasValue || VerifiedAt.HasValue)
        {
            throw new CannotVerifyCustomerException(Id);
        }

        VerifiedAt = verifiedAt;
    }

    public void Lock(string notes = null)
    {
        IsActive = false;
        Notes = notes?.Trim();
        AddDomainEvent(new CustomerLocked(Id));
    }

    public void Unlock(string notes = null)
    {
        IsActive = true;
        Notes = notes?.Trim();
        AddDomainEvent(new CustomerUnlocked(Id));
    }
}
