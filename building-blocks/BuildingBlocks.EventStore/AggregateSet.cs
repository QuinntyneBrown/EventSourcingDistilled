using BuildingBlocks.Abstractions;
using BuildingBlocks.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BuildingBlocks.Abstractions.AggregateRoot;

namespace BuildingBlocks.EventStore
{

    public class AggregateSet : IAggregateSet
    {
        private readonly IEventStoreDbContext _context;
        private readonly IDateTime _dateTime;
        public AggregateSet(IEventStoreDbContext context, IDateTime dateTime)
        {
            _context = context;
            _dateTime = dateTime;
        }

        private IQueryable<StoredEvent> StoredEvents(string aggregateName, Guid[] streamIds = null, DateTime? createdSince = null)
        {
            createdSince ??= _dateTime.UtcNow;

            return from storedEvent in _context.StoredEvents

                   let ids = streamIds != null ? streamIds : _context.StoredEvents.Where(x => x.Aggregate == aggregateName).Select(x => x.StreamId).AsEnumerable()

                   where ids.Contains(storedEvent.StreamId) && storedEvent.CreatedOn <= createdSince

                   select storedEvent;
        }

        public IQueryable<TAggregateRoot> Set<TAggregateRoot>(List<Guid> ids = null)
            where TAggregateRoot : AggregateRoot
        {

            var aggregateName = typeof(TAggregateRoot).Name;

            return (from storedEvent in StoredEvents(aggregateName, ids?.ToArray()).AsEnumerable()
                    group storedEvent by storedEvent.StreamId into storedEventsGroup
                    orderby storedEventsGroup.Key
                    select storedEventsGroup).Aggregate(new List<TAggregateRoot>(), Reduce).AsQueryable();

            static List<TAggregateRoot> Reduce(List<TAggregateRoot> aggregates, IGrouping<Guid, StoredEvent> group)
            {
                var aggregate = Create<TAggregateRoot>();

                group.OrderBy(x => x.CreatedOn)
                    .ForEach(x => aggregate.Apply(JsonConvert.DeserializeObject(x.Data, Type.GetType(x.DotNetType))));

                aggregate.ClearChanges();

                aggregates.Add(aggregate);

                return aggregates;
            }
        }

        public async Task<TAggregateRoot> FindAsync<TAggregateRoot>(Guid streamId)
            where TAggregateRoot : AggregateRoot
        {
            var storedEvents = StoredEvents(typeof(TAggregateRoot).Name, new[] { streamId });

            return storedEvents.Any() ? storedEvents.OrderBy(x => x.CreatedOn).Aggregate(Create<TAggregateRoot>(), Reduce)
                : null;

            static TAggregateRoot Reduce(TAggregateRoot aggregateRoot, StoredEvent storedEvent)
            {
                aggregateRoot.Apply(JsonConvert.DeserializeObject(storedEvent.Data, Type.GetType(storedEvent.DotNetType)));

                aggregateRoot.ClearChanges();

                return aggregateRoot;
            }
        }
    }
}
