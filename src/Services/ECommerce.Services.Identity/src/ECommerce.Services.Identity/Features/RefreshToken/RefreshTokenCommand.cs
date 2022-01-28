using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Jwt;
using ECommerce.Services.Identity.Features.GenerateJwtToken;
using ECommerce.Services.Identity.Features.GenerateRefreshToken;
using ECommerce.Services.Identity.Share.Core.Exceptions;
using ECommerce.Services.Identity.Share.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Services.Identity.Features.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<RefreshTokenCommandResult>;

internal class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, RefreshTokenCommandResult>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly IJwtTokenValidator _tokenValidator;
    private readonly UserManager<ApplicationUser> _userManager;

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

        var userId = userClaimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.NameId);

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
