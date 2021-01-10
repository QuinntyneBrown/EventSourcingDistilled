﻿using BuildingBlocks.Abstractions;
using BuildingBlocks.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcingDistilled.Core.Data
{
    public class EventSourcingDistilledDbContext : BuildingBlocks.Abstractions.IAppDbContext
    {
        private readonly BuildingBlocks.EventStore.IEventStore _eventStore;
        private readonly IAggregateSet _aggregateSet;
        public EventSourcingDistilledDbContext(BuildingBlocks.EventStore.IEventStore eventStore, IAggregateSet aggregateSet)
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
