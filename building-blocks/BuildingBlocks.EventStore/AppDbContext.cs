using BuildingBlocks.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.EventStore
{
    public class AppDbContext : IAppDbContext
    {
        private readonly IEventStore _eventStore;
        private readonly IAggregateSet _aggregateSet;
        public AppDbContext(IEventStore eventStore, IAggregateSet aggregateSet)
        {
            _eventStore = eventStore;
            _aggregateSet = aggregateSet;
        }
        public async Task<TAggregateRoot> FindAsync<TAggregateRoot>(Guid id) where TAggregateRoot : AggregateRoot
            => await _eventStore.LoadAsync<TAggregateRoot>(id);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => await _eventStore.SaveChangesAsync(cancellationToken);

        public IQueryable<T> Set<T>(List<Guid> ids = null) where T : AggregateRoot
            => _aggregateSet.Set<T>(ids);
    }
}
