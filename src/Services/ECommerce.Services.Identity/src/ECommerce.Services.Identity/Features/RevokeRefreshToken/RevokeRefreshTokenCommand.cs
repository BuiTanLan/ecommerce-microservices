using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Identity.Features.RefreshToken;
using ECommerce.Services.Identity.Share.Core.Exceptions;
using ECommerce.Services.Identity.Share.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.Identity.Features.RevokeRefreshToken;

public record RevokeRefreshTokenCommand(string RefreshToken) : ICommand;

internal class RevokeRefreshTokenCommandHandler : ICommandHandler<RevokeRefreshTokenCommand>
{
    private readonly IdentityContext _context;

    public RevokeRefreshTokenCommandHandler(IdentityContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(
        RevokeRefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(RevokeRefreshTokenCommand));

        var refreshToken = await _context.Set<global::ECommerce.Services.Identity.Share.Core.Models.RefreshToken>()
            .SingleOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken: cancellationToken);

        if (refreshToken == null)
            throw new RefreshTokenNotFoundException();

        if (refreshToken.IsRefreshTokenValid() == false)
            throw new InvalidRefreshTokenException();

        // revoke token and save
        refreshToken.RevokedAt = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
