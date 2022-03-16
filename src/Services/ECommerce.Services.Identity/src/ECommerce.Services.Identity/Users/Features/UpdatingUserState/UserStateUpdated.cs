using ECommerce.Services.Identity.Shared.Models;
using MicroBootstrap.Core.Domain.Events.External;

namespace ECommerce.Services.Identity.Users.Features.UpdatingUserState;

public record UserStateUpdated(Guid UserId, UserState OldUserState, UserState NewUserState) : IntegrationEvent;
