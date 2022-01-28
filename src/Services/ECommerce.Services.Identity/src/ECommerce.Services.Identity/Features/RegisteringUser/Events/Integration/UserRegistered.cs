using BuildingBlocks.Core.Domain.Events.External;
using ECommerce.Services.Identity.Share.Core.Models;

namespace ECommerce.Services.Identity.Features.RegisteringUser.Events.Integration;

public record UserRegistered(
    Guid IdentityId,
    string Email,
    string FirstName,
    string LastName,
    UserState UserState,
    DateTime CreatedAt,
    List<string> Roles) : IntegrationEvent;
