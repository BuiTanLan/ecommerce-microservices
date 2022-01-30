using BuildingBlocks.Core.Domain.Events.External;
using ECommerce.Services.Identity.Shared.Models;

namespace ECommerce.Services.Identity.Users.Features.UpdatingUserState;

public record UserStateUpdated(Guid UserId, UserState OldUserState, UserState NewUserState) : IntegrationEvent;
