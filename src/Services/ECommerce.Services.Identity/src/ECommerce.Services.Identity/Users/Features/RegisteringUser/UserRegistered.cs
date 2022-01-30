using BuildingBlocks.Core.Domain.Events.External;

namespace ECommerce.Services.Identity.Users.Features.RegisteringUser;

public record UserRegistered(
    Guid IdentityId,
    string Email,
    string FirstName,
    string LastName,
    List<string> Roles) : IntegrationEvent;
