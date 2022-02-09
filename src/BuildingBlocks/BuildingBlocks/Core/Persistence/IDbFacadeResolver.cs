using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BuildingBlocks.Core.Persistence;

public interface IDbFacadeResolver
{
    DatabaseFacade Database { get; }
}
