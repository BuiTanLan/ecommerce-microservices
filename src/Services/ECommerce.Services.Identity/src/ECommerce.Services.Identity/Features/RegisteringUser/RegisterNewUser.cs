using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Exception;
using BuildingBlocks.Messaging.Outbox;
using ECommerce.Services.Identity.Features.RegisteringUser.Events.Integration;
using ECommerce.Services.Identity.Share.Core;
using ECommerce.Services.Identity.Share.Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Services.Identity.Features.RegisteringUser;

public record RegisterNewUser(
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword,
    List<string> Roles = null) : ICreateCommand<RegisterNewUserResult>
{
    public DateTime CreatedAt { get; init; } = DateTime.Now;
}

internal class RegisterNewUserValidator : AbstractValidator<RegisterNewUser>
{
    public RegisterNewUserValidator()
    {
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
    }
}

internal class RegisterNewUserHandler : ICommandHandler<RegisterNewUser, RegisterNewUserResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOutboxService _outboxService;

    public RegisterNewUserHandler(UserManager<ApplicationUser> userManager, IOutboxService outboxService)
    {
        _userManager = Guard.Against.Null(userManager, nameof(userManager));
        _outboxService = Guard.Against.Null(outboxService, nameof(outboxService));
    }

    public async Task<RegisterNewUserResult> Handle(RegisterNewUser request, CancellationToken cancellationToken)
    {
        if (request.Roles != null &&
            request.Roles.All(x => x.Contains(Constants.Role.Admin, StringComparison.Ordinal) ||
                                   x.Contains(Constants.Role.User, StringComparison.Ordinal)) == false)
        {
            throw new AppException("Invalid roles.");
        }

        Guard.Against.NullOrEmpty(request.Password, nameof(request.Password));
        Guard.Against.NullOrEmpty(request.ConfirmPassword, nameof(request.ConfirmPassword));
        Guard.Against.NullOrEmpty(request.Email, nameof(request.Email));
        Guard.Against.NullOrEmpty(request.FirstName, nameof(request.FirstName));
        Guard.Against.NullOrEmpty(request.LastName, nameof(request.LastName));

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

        await _outboxService.SaveAsync(cancellationToken, new UserRegistered(
            applicationUser.Id,
            applicationUser.Email,
            applicationUser.FirstName,
            applicationUser.LastName,
            applicationUser.UserState,
            request.CreatedAt,
            request.Roles));

        return new RegisterNewUserResult(new RegisterIdentityUserDto
        {
            Id = applicationUser.Id,
            Email = applicationUser.Email,
            Username = applicationUser.UserName,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            Roles = request.Roles ?? new List<string> { Constants.Role.User },
            CreatedAt = request.CreatedAt,
            UserState = applicationUser.UserState
        });
    }
}

internal record RegisterNewUserResult(RegisterIdentityUserDto User);
