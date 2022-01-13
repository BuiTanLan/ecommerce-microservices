using System;
using System.Threading.Tasks;

namespace BuildingBlocks.EFCore;

public interface IUnitOfWork : IDisposable
{
    Task<bool> CommitAsync();
}
