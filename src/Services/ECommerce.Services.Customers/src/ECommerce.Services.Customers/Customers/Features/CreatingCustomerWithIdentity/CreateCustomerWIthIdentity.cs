using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.ValueObjects;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Exception;
using BuildingBlocks.IdsGenerator;
using ECommerce.Services.Customers.Customers.Models;
using ECommerce.Services.Customers.Identity.Exceptions;
using ECommerce.Services.Customers.Shared.Clients.Identity;
using ECommerce.Services.Customers.Shared.Clients.Identity.Dtos;
using ECommerce.Services.Customers.Shared.Data;
using ECommerce.Services.Customers.Shared.ValueObjects;

namespace ECommerce.Services.Customers.Customers.Features.CreatingCustomerWithIdentity;

public record CreateCustomerWIthIdentity(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string Password) : ICreateCommand<CreateCustomerWithIdentityResult>
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}

internal class CreateCustomerWIthIdentityValidator : AbstractValidator<CreateCustomerWIthIdentity>
{
    public CreateCustomerWIthIdentityValidator()
    {
        CascadeMode = CascadeMode.Stop;

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
internal class CreateCustomerWIthIdentityHandler : ICommandHandler<CreateCustomerWIthIdentity, CreateCustomerWithIdentityResult>
{
    private readonly IIdentityApiClient _identityApiClient;
    private readonly CustomersDbContext _customersDbContext;
    private readonly ILogger<CreateCustomerWIthIdentityHandler> _logger;

    public CreateCustomerWIthIdentityHandler(
        IIdentityApiClient identityApiClient,
        CustomersDbContext customersDbContext,
        ILogger<CreateCustomerWIthIdentityHandler> logger)
    {
        _identityApiClient = identityApiClient;
        _customersDbContext = customersDbContext;
        _logger = logger;
    }

    public async Task<CreateCustomerWithIdentityResult> Handle(CreateCustomerWIthIdentity command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var identityUser = (await _identityApiClient.GetUserByEmailAsync(command.Email, cancellationToken))
            ?.UserIdentity;

        if (identityUser is null)
        {
            identityUser = (await _identityApiClient.CreateUserIdentityAsync(
                new CreateUserRequest(
                    command.UserName,
                    command.Email,
                    command.FirstName,
                    command.LastName,
                    command.Password,
                    command.Password),
                cancellationToken))?.UserIdentity;
        }

        Guard.Against.NotFound(identityUser, new IdentityUserNotFoundException(command.Email));

        var customer = Customer.Create(
            command.Id,
            Email.Create(identityUser.Email),
            CustomerName.Create(identityUser.FirstName, identityUser.LastName),
            identityUser.Id);

        await _customersDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created a customer with ID: '{@CustomerId}'", customer.Id);

        return new CreateCustomerWithIdentityResult(
            customer.Id,
            customer.Email!,
            customer.Name.FirstName,
            customer.Name.LastName,
            customer.IdentityId);
    }
}

public record CreateCustomerWithIdentityResult(
    long CustomerId,
    string Email,
    string FirstName,
    string LastName,
    Guid IdentityUserId);
