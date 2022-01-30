using Ardalis.GuardClauses;
using BuildingBlocks.Core.ValueObjects;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.IdsGenerator;
using ECommerce.Services.Customers.Customers.Clients;
using ECommerce.Services.Customers.Customers.Clients.Dtos;
using ECommerce.Services.Customers.Customers.Models;
using ECommerce.Services.Customers.Customers.ValueObjects;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Customers.Customers.Features.CreatingCustomer;

public record CreateCustomer(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string Password) : ICreateCommand<CreateCustomerResult>;

internal class CreateCustomerValidator : AbstractValidator<CreateCustomer>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.FirstName)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.UserName)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email address is invalid.");
    }
}

// Synchronous integration for creating customer
internal class CreateCustomerHandler : ICommandHandler<CreateCustomer, CreateCustomerResult>
{
    private readonly IIdentityApiClient _identityApiClient;
    private readonly CustomersDbContext _customersDbContext;
    private readonly ILogger<CreateCustomerHandler> _logger;

    public CreateCustomerHandler(
        IIdentityApiClient identityApiClient,
        CustomersDbContext customersDbContext,
        ILogger<CreateCustomerHandler> logger)
    {
        _identityApiClient = identityApiClient;
        _customersDbContext = customersDbContext;
        _logger = logger;
    }

    public async Task<CreateCustomerResult> Handle(CreateCustomer command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var identityUser = (await _identityApiClient.CreateUserIdentityAsync(
            new CreateUserRequest(
                command.UserName,
                command.Email,
                command.FirstName,
                command.LastName,
                command.Password),
            cancellationToken))?.UserIdentity;

        if (identityUser == null)
            throw new CreatingUserForCustomerException();

        var customer = Customer.Create(
            SnowFlakIdGenerator.NewId(),
            new Email(identityUser.Email),
            new Name(identityUser.FirstName, identityUser.LastName),
            identityUser.Id);

        await _customersDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created a customer with ID: '{@CustomerId}'", customer.Id);

        return new CreateCustomerResult(
            customer.Id,
            customer.Email!,
            customer.Name.FirstName,
            customer.Name.LastName,
            customer.IdentityId);
    }
}

public record CreateCustomerResult(
    long CustomerId,
    string Email,
    string FirstName,
    string LastName,
    Guid IdentityUserId);
