﻿using BuildingBlocks.Abstractions;
using BuildingBlocks.EventStore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcingDistilled.Core.Data
{
    public class EventSourcingDistilledDbContext: IAppDbContext
    {
        private readonly IEventStore _eventStore;
        private readonly IAggregateSet _aggregateSet;
        public EventSourcingDistilledDbContext(IEventStore eventStore, IAggregateSet aggregateSet)
        {
            _eventStore = eventStore;
            _aggregateSet = aggregateSet;
        }
        public IQueryable<TAggregateRoot> Set<TAggregateRoot>()
            where TAggregateRoot: AggregateRoot
            => _aggregateSet.Set<TAggregateRoot>();

        public void Store(AggregateRoot aggregateRoot)
            => _eventStore.Save(aggregateRoot);

        public TAggregateRoot Find<TAggregateRoot>(Guid id)
            where TAggregateRoot : AggregateRoot
            => _aggregateSet.Find<TAggregateRoot>(id);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => await _eventStore.SaveChangesAsync(cancellationToken);
    }
}
