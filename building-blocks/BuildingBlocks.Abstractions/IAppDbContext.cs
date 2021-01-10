using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Abstractions
{
    public interface IAppDbContext
    {
        IQueryable<T> Set<T>(List<Guid> ids = null)
            where T : AggregateRoot;

        Task<TAggregateRoot> FindAsync<TAggregateRoot>(Guid id)
            where TAggregateRoot : AggregateRoot;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
