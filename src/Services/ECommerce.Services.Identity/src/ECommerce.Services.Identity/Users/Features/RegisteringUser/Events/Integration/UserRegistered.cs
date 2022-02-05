using BuildingBlocks.Core.Domain.Events.External;

namespace ECommerce.Services.Identity.Users.Features.RegisteringUser.Events.Integration;

public record UserRegistered(
    Guid IdentityId,
    string Email,
    string UserName,
    string FirstName,
    string LastName,
    List<string>? Roles) : IntegrationEvent;
