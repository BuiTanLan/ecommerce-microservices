using System.Security.Claims;
using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Jwt;
using BuildingBlocks.Utils;
using Identity.Core.Exceptions;
using Identity.Core.Models;
using Identity.Features.GenerateJwtToken;
using Identity.Features.GenerateRefreshToken;
using Microsoft.AspNetCore.Identity;

namespace Identity.Features.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<RefreshTokenCommandResult>;

internal class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, RefreshTokenCommandResult>
{
    private readonly IJwtTokenValidator _tokenValidator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICommandProcessor _commandProcessor;

    public RefreshTokenCommandHandler(IJwtTokenValidator tokenValidator, UserManager<ApplicationUser> userManager,
        ICommandProcessor commandProcessor)
    {
        _tokenValidator = tokenValidator;
        _userManager = userManager;
        _commandProcessor = commandProcessor;
    }

    public async Task<RefreshTokenCommandResult> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(RefreshTokenCommand));

        // invalid token/signing key was passed and we can't extract user claims
        var userClaimsPrincipal = _tokenValidator.GetPrincipalFromToken(request.AccessToken);

        if (userClaimsPrincipal is null)
            throw new InvalidTokenException();

        string userId = userClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        var identityUser = await _userManager.FindByIdAsync(userId);

        if (identityUser == null)
            throw new UserNotFoundException(userId);

        var refreshToken =
            (await _commandProcessor.SendAsync(
                new GenerateRefreshTokenCommand { UserId = identityUser.Id, Token = request.RefreshToken },
                cancellationToken)).RefreshToken;

        var jsonWebToken =
            (await _commandProcessor.SendAsync(new GenerateJwtTokenCommand(identityUser, refreshToken.Token),
                cancellationToken)).JsonWebToken;

        return new RefreshTokenCommandResult(identityUser, jsonWebToken, refreshToken.Token);
    }
}
