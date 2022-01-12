using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.CQRS.Query;
using BuildingBlocks.Domain;
using BuildingBlocks.Jwt;
using Identity.Core.Exceptions;
using Identity.Core.Models;
using Identity.Features.GenerateJwtToken;
using Identity.Features.GenerateRefreshToken;
using Identity.Features.Login.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.Features.Login;

public record LoginCommand(string UserNameOrEmail, string Password, bool Remember) :
    ICommand<LoginCommandResponse>, ITxRequest;

public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginCommandResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IJwtHandler _jwtHandler;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly JwtOptions _jwtOptions;

    public LoginCommandHandler(UserManager<ApplicationUser> userManager,
        ICommandProcessor commandProcessor,
        IQueryProcessor queryProcessor,
        IJwtHandler jwtHandler,
        IOptions<JwtOptions> jwtOptions,
        SignInManager<ApplicationUser> signInManager,
        ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager;
        _commandProcessor = commandProcessor;
        _queryProcessor = queryProcessor;
        _jwtHandler = jwtHandler;
        _signInManager = signInManager;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<LoginCommandResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(LoginCommand));

        var identityUser = await _userManager.FindByNameAsync(request.UserNameOrEmail) ??
                           await _userManager.FindByEmailAsync(request.UserNameOrEmail);

        if (identityUser == null)
        {
            throw new UserNotFoundException(request.UserNameOrEmail);
        }


        SignInResult signinResult = await _signInManager.PasswordSignInAsync(
            request.UserNameOrEmail,
            request.Password,
            isPersistent: request.Remember,
            lockoutOnFailure: false);


        if (signinResult.IsNotAllowed)
        {
            if (!await _userManager.IsEmailConfirmedAsync(identityUser))
            {
                throw new EmailNotConfirmedException(identityUser.Email);
            }

            if (!await _userManager.IsPhoneNumberConfirmedAsync(identityUser))
            {
                throw new PhoneNumberNotConfirmedException(identityUser.PhoneNumber);
            }
        }
        else if (signinResult.IsLockedOut)
        {
            throw new UserLockedException(identityUser.Id.ToString());
        }
        else if (signinResult.RequiresTwoFactor)
        {
            throw new RequiresTwoFactorException("Require two factor authentication.");
        }
        else if (signinResult.Succeeded == false)
        {
            throw new PasswordIsInvalidException();
        }

        var refreshToken =
            (await _commandProcessor.SendAsync(new GenerateRefreshTokenCommand { UserId = identityUser.Id },
                cancellationToken)).RefreshToken;

        var jsonWebToken =
            (await _commandProcessor.SendAsync(new GenerateJwtTokenCommand(identityUser, refreshToken.Token),
                cancellationToken)).JsonWebToken;

        _logger.LogInformation("User with ID: {ID} has been authenticated", identityUser.Id);

        // we can don't return value from command and get token from a short term session in our request with `TokenStorageService`
        return new LoginCommandResponse(identityUser, jsonWebToken, refreshToken.Token);
    }
}
