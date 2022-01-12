using System.Security.Cryptography;
using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Utils;
using Identity.Core.Dtos;
using Identity.Features.RefreshToken;
using Identity.Infrastructure.Data;
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

    public async Task<GenerateRefreshTokenCommandResult> Handle(GenerateRefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(GenerateRefreshTokenCommand));

        var refreshToken = await _context.Set<Core.Models.RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.UserId == request.UserId && rt.Token == request.Token,
                cancellationToken: cancellationToken);

        if (refreshToken == null)
        {
            string token = GetRefreshToken();

            refreshToken = new Core.Models.RefreshToken
            {
                UserId = request.UserId,
                Token = token,
                CreatedAt = DateTime.Now,
                ExpiredAt = DateTime.Now.AddDays(30),
                CreatedByIp = IpHelper.GetIpAddress(),
            };

            await _context.Set<Core.Models.RefreshToken>().AddAsync(refreshToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            if (IsRefreshTokenValid(refreshToken) == false)
                throw new InvalidRefreshTokenException();

            string token = GetRefreshToken();

            refreshToken.Token = token;
            refreshToken.ExpiredAt = DateTime.Now;
            refreshToken.CreatedAt = DateTime.Now.AddDays(10);
            refreshToken.CreatedByIp = IpHelper.GetIpAddress();

            _context.Set<Core.Models.RefreshToken>().Update(refreshToken);
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

    private string GetRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);

        string refreshToken = Convert.ToBase64String(randomNumber);

        return refreshToken;
    }

    private bool IsRefreshTokenValid(Core.Models.RefreshToken existingToken, double? ttlRefreshToken = null)
    {
        // Token already expired or revoked, then return false
        if (existingToken.IsActive == false)
        {
            return false;
        }

        if (ttlRefreshToken is not null && existingToken.CreatedAt.AddDays((long)ttlRefreshToken) <= DateTime.Now)
        {
            return false;
        }

        return true;
    }

    private async Task RemoveOldRefreshTokens(Guid userId, long? ttlRefreshToken = null)
    {
        var refreshTokens = _context.Set<Core.Models.RefreshToken>().Where(rt => rt.UserId == userId);

        refreshTokens.ToList().RemoveAll(x => IsRefreshTokenValid(x, ttlRefreshToken) == false);

        await _context.SaveChangesAsync();
    }
}
