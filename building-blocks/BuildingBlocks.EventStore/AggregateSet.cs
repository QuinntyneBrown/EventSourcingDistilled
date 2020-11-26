using BuildingBlocks.Abstractions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BuildingBlocks.EventStore
{
    public interface IAggregateSet
    {
        IQueryable<TAggregateRoot> Set<TAggregateRoot>()
            where TAggregateRoot : AggregateRoot;

        Task<TAggregateRoot> FindAsync<TAggregateRoot>(Guid id)
            where TAggregateRoot : AggregateRoot;
    }

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

            IEnumerable<Guid> ids = streamIds != null
                ? streamIds
                : (from storedEvent in _context.StoredEvents
                    where storedEvent.Aggregate == aggregateName
                    select storedEvent.StreamId).Distinct();

            return from storedEvent in _context.StoredEvents
                    where ids.Contains(storedEvent.StreamId) && storedEvent.CreatedOn <= createdSince
                    select storedEvent;
        }

        public IQueryable<TAggregateRoot> Set<TAggregateRoot>()
            where TAggregateRoot : AggregateRoot
        {
            var aggregates = new List<TAggregateRoot>();

            var aggregateName = typeof(TAggregateRoot).Name;

            var storedEventsGroups = from storedEvent in StoredEvents(aggregateName).ToList()
                                    group storedEvent by storedEvent.StreamId into storedEventsGroup
                                    orderby storedEventsGroup.Key
                                    select storedEventsGroup;

            foreach (var storedEventGroup in storedEventsGroups)
            {
                var aggregate = (TAggregateRoot)FormatterServices.GetUninitializedObject(typeof(TAggregateRoot));

                foreach (var storedEvent in storedEventGroup.OrderBy(x => x.Sequence))
                {
                    aggregate.Apply(JsonConvert.DeserializeObject(storedEvent.Data, Type.GetType(storedEvent.DotNetType)));
                }

                aggregates.Add(aggregate);
            }

            return aggregates.AsQueryable();
        }

        public async Task<TAggregateRoot> FindAsync<TAggregateRoot>(Guid streamId)
            where TAggregateRoot : AggregateRoot
        {
            var aggregate = (TAggregateRoot)FormatterServices.GetUninitializedObject(typeof(TAggregateRoot));

            var storedEvents = StoredEvents(typeof(TAggregateRoot).Name, new Guid[1] { streamId }).ToList();

            foreach (var storedEvent in storedEvents.OrderBy(x => x.Sequence))
            {
                aggregate.Apply(JsonConvert.DeserializeObject(storedEvent.Data, Type.GetType(storedEvent.DotNetType)));
            }

            return aggregate;
        }
    }
}
