using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Identity.Share.Core.Models;

namespace ECommerce.Services.Identity.Features.UpdatingUserState;

public record UpdateUserState(Guid UserId, UserState State) : ICommand;
