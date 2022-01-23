using System.Security.Cryptography;
using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Utils;
using Identity.Features.RefreshToken;
using Identity.Share.Core.Dtos;
using Identity.Share.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Features.GenerateRefreshToken;

public record GenerateRefreshTokenCommand : ICommand<GenerateRefreshTokenCommandResult>
{
    public Guid UserId { get; init; }
    public string Token { get; init; }
}

public class
    GenerateRefreshTokenCommandHandler : ICommandHandler<GenerateRefreshTokenCommand, GenerateRefreshTokenCommandResult>
{
    private readonly IdentityContext _context;

    public GenerateRefreshTokenCommandHandler(IdentityContext context)
    {
        _context = context;
    }

    public async Task<GenerateRefreshTokenCommandResult> Handle(
        GenerateRefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(GenerateRefreshTokenCommand));

        var refreshToken = await _context.Set<Share.Core.Models.RefreshToken>()
            .FirstOrDefaultAsync(
                rt => rt.UserId == request.UserId && rt.Token == request.Token,
                cancellationToken);

        if (refreshToken == null)
        {
            var token = Share.Core.Models.RefreshToken.GetRefreshToken();

            refreshToken = new Share.Core.Models.RefreshToken
            {
                UserId = request.UserId,
                Token = token,
                CreatedAt = DateTime.Now,
                ExpiredAt = DateTime.Now.AddDays(30),
                CreatedByIp = IpHelper.GetIpAddress()
            };

            await _context.Set<Share.Core.Models.RefreshToken>().AddAsync(refreshToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            if (!refreshToken.IsRefreshTokenValid())
                throw new InvalidRefreshTokenException();

            var token = Share.Core.Models.RefreshToken.GetRefreshToken();

            refreshToken.Token = token;
            refreshToken.ExpiredAt = DateTime.Now;
            refreshToken.CreatedAt = DateTime.Now.AddDays(10);
            refreshToken.CreatedByIp = IpHelper.GetIpAddress();

            _context.Set<Share.Core.Models.RefreshToken>().Update(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // remove old refresh tokens from user
        // we could also maintain them on the database with changing their revoke date
        await RemoveOldRefreshTokens(request.UserId);

        return new GenerateRefreshTokenCommandResult(new RefreshTokenDto
        {
            Token = refreshToken.Token,
            CreatedAt = refreshToken.CreatedAt,
            ExpireAt = refreshToken.ExpiredAt,
            UserId = refreshToken.UserId,
            CreatedByIp = refreshToken.CreatedByIp,
            IsActive = refreshToken.IsActive,
            IsExpired = refreshToken.IsExpired,
            IsRevoked = refreshToken.IsRevoked,
            RevokedAt = refreshToken.RevokedAt
        });
    }


    private async Task RemoveOldRefreshTokens(Guid userId, long? ttlRefreshToken = null)
    {
        var refreshTokens = _context.Set<Share.Core.Models.RefreshToken>().Where(rt => rt.UserId == userId);

        refreshTokens.ToList().RemoveAll(x => !x.IsRefreshTokenValid(ttlRefreshToken));

        await _context.SaveChangesAsync();
    }
}
