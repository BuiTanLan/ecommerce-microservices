using BuildingBlocks.Core.Domain.Events.External;

namespace ECommerce.Services.Customers.Customers.Features.RegisteringUser.Events.External;

public record UserRegistered(
    Guid IdentityId,
    string Email,
    string FirstName,
    string LastName,
    UserState UserState,
    DateTime CreatedAt,
    List<string> Roles) : IntegrationEvent;
