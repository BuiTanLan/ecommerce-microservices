using Ardalis.GuardClauses;
using ECommerce.Services.Identity.Shared.Models;
using ECommerce.Services.Identity.Users.Dtos;
using ECommerce.Services.Identity.Users.Features.RegisteringUser.Events.Integration;
using FluentValidation;
using MicroBootstrap.Abstractions.Core.Domain.Events;
using MicroBootstrap.Abstractions.CQRS.Command;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Services.Identity.Users.Features.RegisteringUser;

public record RegisterUser(
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword,
    List<string>? Roles = null) : ITxCreateCommand<RegisterUserResult>
{
    public DateTime CreatedAt { get; init; } = DateTime.Now;
}

internal class RegisterUserValidator : AbstractValidator<RegisterUser>
{
    public RegisterUserValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(v => v.FirstName)
            .NotEmpty()
            .WithMessage("FirstName is required.");

        RuleFor(v => v.LastName)
            .NotEmpty()
            .WithMessage("LastName is required.");

        RuleFor(v => v.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress();

        RuleFor(v => v.UserName)
            .NotEmpty()
            .WithMessage("UserName is required.");

        RuleFor(v => v.Password)
            .NotEmpty()
            .WithMessage("Password is required.");

        RuleFor(v => v.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("The password and confirmation password do not match.")
            .NotEmpty();

        RuleFor(v => v.Roles).Custom((roles, c) =>
        {
            if (roles != null &&
                roles.All(x => x.Contains(Constants.Role.Admin, StringComparison.Ordinal) ||
                               x.Contains(Constants.Role.User, StringComparison.Ordinal)) == false)
            {
                c.AddFailure("Invalid roles.");
            }
        });
    }
}

// using transaction script instead of using domain business logic here
// https://www.youtube.com/watch?v=PrJIMTZsbDw
internal class RegisterUserHandler : ICommandHandler<RegisterUser, RegisterUserResult>
{
    private readonly IEventProcessor _eventProcessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterUserHandler(UserManager<ApplicationUser> userManager, IEventProcessor eventProcessor)
    {
        _userManager = Guard.Against.Null(userManager, nameof(userManager));
        _eventProcessor = Guard.Against.Null(eventProcessor, nameof(eventProcessor));
    }

    public async Task<RegisterUserResult> Handle(RegisterUser request, CancellationToken cancellationToken)
    {
        var applicationUser = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            Email = request.Email,
            UserState = UserState.Active,
            CreatedAt = request.CreatedAt,
        };

        var identityResult = await _userManager.CreateAsync(applicationUser, request.Password);
        var roleResult = await _userManager.AddToRolesAsync(
            applicationUser,
            request.Roles ?? new List<string> { Constants.Role.User });

        if (identityResult.Succeeded == false)
            throw new RegisterIdentityUserException(string.Join(',', identityResult.Errors.Select(e => e.Description)));

        if (roleResult.Succeeded == false)
            throw new RegisterIdentityUserException(string.Join(',', roleResult.Errors.Select(e => e.Description)));

        // publish our integration event and save to outbox should do in same transaction of our business logic actions. we could use TxBehaviour or ITxDbContextExecutes interface
        // This service is not DDD, so we couldn't use DomainEventPublisher to publish mapped integration events
        await _eventProcessor.PublishAsync(
            new UserRegistered(
                applicationUser.Id,
                applicationUser.Email,
                applicationUser.UserName,
                applicationUser.FirstName,
                applicationUser.LastName,
                request.Roles),
            cancellationToken);

        return new RegisterUserResult(new IdentityUserDto
        {
            Id = applicationUser.Id,
            Email = applicationUser.Email,
            UserName = applicationUser.UserName,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            Roles = request.Roles ?? new List<string> { Constants.Role.User },
            RefreshTokens = applicationUser?.RefreshTokens?.Select(x => x.Token),
            CreatedAt = request.CreatedAt,
            UserState = UserState.Active
        });
    }
}

internal record RegisterUserResult(IdentityUserDto? UserIdentity);
