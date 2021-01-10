using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Newtonsoft.Json.JsonConvert;
using static BuildingBlocks.Abstractions.AggregateRoot;

namespace BuildingBlocks.EventStore
{
    public class EventStore : IEventStore
    {
        private readonly List<StoredEvent> _changes = new List<StoredEvent>();
        private readonly IDateTime _dateTime;
        private readonly IEventStoreDbContext _context;
        private readonly ICorrelationIdAccessor _correlationIdAccessor;
        private List<AggregateRoot> _trackedAggregates = new List<AggregateRoot>();
        public EventStore(IEventStoreDbContext context, IDateTime dateTime, ICorrelationIdAccessor correlationIdAccessor)
        {
            _dateTime = dateTime;
            _context = context;
            _correlationIdAccessor = correlationIdAccessor;
        }

        public void Store(AggregateRoot aggregateRoot)
        {
            var type = aggregateRoot.GetType();

            _changes.AddRange(aggregateRoot.DomainEvents
                .Select(@event => @event.ToStoredEvent(_dateTime, aggregateRoot, _correlationIdAccessor.CorrelationId)));

            aggregateRoot.ClearChanges();
        }

        public async Task<TAggregateRoot> LoadAsync<TAggregateRoot>(Guid id)
            where TAggregateRoot : AggregateRoot
        {
            var events = (await _context.StoredEvents.Where(x => x.StreamId == id).OrderBy(x => x.CreatedOn).ToListAsync())
                    .Select(x => DeserializeObject(x.Data, Type.GetType(x.DotNetType)));

            if (!events.Any())
                return null;

            var aggregate = events.Aggregate(Create<TAggregateRoot>(), (x, y) => x.Apply(y) as TAggregateRoot);

            aggregate.ClearChanges();

            _trackedAggregates.Add(aggregate);

            return aggregate;
        }

        public void Add(AggregateRoot aggregateRoot)
            => _trackedAggregates.Add(aggregateRoot);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            foreach (var aggregateRoot in _trackedAggregates)
            {
                var type = aggregateRoot.GetType();

                _context.StoredEvents.AddRange(aggregateRoot.DomainEvents
                    .Select(@event => @event.ToStoredEvent(_dateTime, aggregateRoot, _correlationIdAccessor.CorrelationId)));
            }

            _trackedAggregates.Clear();

            return await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public static class EventExtensions
    {
        public static StoredEvent ToStoredEvent(this object @event, IDateTime dateTime, AggregateRoot aggregateRoot, Guid correlationId)
        {
            var type = aggregateRoot.GetType();

            return new StoredEvent
            {
                StoredEventId = Guid.NewGuid(),
                Aggregate = aggregateRoot.GetType().Name,
                AggregateDotNetType = aggregateRoot.GetType().AssemblyQualifiedName,
                Data = SerializeObject(@event),
                StreamId = (Guid)type.GetProperty($"{type.Name}Id").GetValue(aggregateRoot, null),
                DotNetType = @event.GetType().AssemblyQualifiedName,
                Type = @event.GetType().Name,
                CreatedOn = dateTime.UtcNow,
                Sequence = 0,
                CorrelationId = correlationId
            };
        }
    }
}
