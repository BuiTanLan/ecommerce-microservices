using BuildingBlocks.Core.Utils;
using BuildingBlocks.Exception;
using BuildingBlocks.Exception.Types;
using ECommerce.Services.Identity.Shared.Models;

namespace ECommerce.Services.Identity.Users.Features.UpdatingUserState;

internal class UserStateCannotBeChangedException : AppException
{
    public UserState State { get; }
    public Guid UserId { get; }

    public UserStateCannotBeChangedException(UserState state, Guid userId)
        : base($"User state cannot be changed to: '{state.ToName()}' for user with ID: '{userId}'.")
    {
        State = state;
        UserId = userId;
    }
}
