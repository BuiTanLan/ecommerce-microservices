using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BuildingBlocks.EFCore
{
    public interface IDbFacadeResolver
    {
        DatabaseFacade Database { get; }
    }
}
