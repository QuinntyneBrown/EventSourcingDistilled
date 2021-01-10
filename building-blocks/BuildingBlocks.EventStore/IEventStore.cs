using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.EventStore
{
    public interface IEventStore
    {
        DbSet<StoredEvent> StoredEvents { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        Task<TAggregateRoot> LoadAsync<TAggregateRoot>(Guid id)
            where TAggregateRoot : AggregateRoot;
        void Add(IAggregateRoot aggregateRoot);
    }
}
