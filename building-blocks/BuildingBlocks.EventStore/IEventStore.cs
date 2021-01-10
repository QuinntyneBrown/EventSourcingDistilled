using BuildingBlocks.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.EventStore
{
    public interface IEventStore
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<TAggregateRoot> LoadAsync<TAggregateRoot>(Guid id)
            where TAggregateRoot : AggregateRoot;
        void Add(AggregateRoot aggregateRoot);
    }
}
