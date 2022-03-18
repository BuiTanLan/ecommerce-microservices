using ECommerce.Services.Identity.Shared.Models;
using MicroBootstrap.Core.Exception.Types;
using MicroBootstrap.Core.Extensions.Utils;

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
