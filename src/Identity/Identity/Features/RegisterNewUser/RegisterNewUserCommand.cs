using BuildingBlocks.CQRS.Command;
using Identity.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Features.RegisterNewUser;

public record RegisterNewUserCommand(string FirstName, string LastName, string UserName, string Email, string Password,
    string ConfirmPassword) : ICommand<RegisterNewUserCommandResult>;

internal class RegisterNewUserCommandHandler : ICommandHandler<RegisterNewUserCommand, RegisterNewUserCommandResult>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterNewUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RegisterNewUserCommandResult> Handle(RegisterNewUserCommand request,
        CancellationToken cancellationToken)
    {
        ApplicationUser applicationUser = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            Email = request.Email
        };

        var identityResult = await _userManager.CreateAsync(applicationUser, request.Password);
        var roleResult = await _userManager.AddToRoleAsync(applicationUser, ApplicationRole.User.Name);

        if (identityResult.Succeeded == false)
            throw new RegisterIdentityUserException(string.Join(',', identityResult.Errors.Select(e => e.Description)));

        if (roleResult.Succeeded == false)
            throw new RegisterIdentityUserException(string.Join(',', roleResult.Errors.Select(e => e.Description)));

        return new RegisterNewUserCommandResult(new RegisterIdentityUserDto
        {
            Id = applicationUser.Id,
            Email = applicationUser.Email,
            Username = applicationUser.UserName,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            IsAdmin = applicationUser.IsAdmin
        });
    }
}
