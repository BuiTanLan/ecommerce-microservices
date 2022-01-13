using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using Identity.Core.Exceptions;
using Identity.Features.RefreshToken;
using Identity.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Features.RevokeRefreshToken;

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

        var refreshToken = await _context.Set<Core.Models.RefreshToken>()
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
