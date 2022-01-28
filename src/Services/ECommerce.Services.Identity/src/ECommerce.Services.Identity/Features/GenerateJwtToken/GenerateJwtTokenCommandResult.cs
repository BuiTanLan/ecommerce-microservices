using BuildingBlocks.Jwt;

namespace ECommerce.Services.Identity.Features.GenerateJwtToken;

public record GenerateJwtTokenCommandResult(JsonWebToken JsonWebToken);
