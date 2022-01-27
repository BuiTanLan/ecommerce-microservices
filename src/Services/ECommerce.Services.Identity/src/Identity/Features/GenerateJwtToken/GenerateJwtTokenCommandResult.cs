using BuildingBlocks.Jwt;

namespace Identity.Features.GenerateJwtToken;

public record GenerateJwtTokenCommandResult(JsonWebToken JsonWebToken);
