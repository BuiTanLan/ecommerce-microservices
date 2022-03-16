using MicroBootstrap.Security.Jwt;

namespace ECommerce.Services.Identity.Identity.Features.GenerateJwtToken;

public record GenerateJwtTokenCommandResult(JsonWebToken JsonWebToken);
