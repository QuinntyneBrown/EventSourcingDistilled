using BuildingBlocks.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BuildingBlocks.EventStore
{
    public interface IAggregateSet
    {
        IQueryable<TAggregateRoot> Set<TAggregateRoot>()
            where TAggregateRoot : AggregateRoot;

        TAggregateRoot Find<TAggregateRoot>(Guid id)
            where TAggregateRoot : AggregateRoot;
    }

    public class AggregateSet : IAggregateSet
    {
        private readonly IEventStoreDbContext _dbContext;
        public AggregateSet(IEventStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IQueryable<StoredEvent> StoredEvents(string aggregateName, Guid[] streamIds = null, DateTime? limit = null)
        {
            IQueryable<Guid> ids = streamIds != null
                ? streamIds.AsQueryable()
                : (from storedEvent in _dbContext.StoredEvents
                   where storedEvent.Aggregate == aggregateName
                   select storedEvent.StreamId).Distinct();

            return from storedEvent in _dbContext.StoredEvents
                   where ids.Contains(storedEvent.StreamId)
                   select storedEvent;
        }

        public IQueryable<TAggregateRoot> Set<TAggregateRoot>()
            where TAggregateRoot : AggregateRoot
        {
            var aggregates = new List<TAggregateRoot>();

            var aggregateName = typeof(TAggregateRoot).Name;

            var storedEventsGroup = from storedEvent in StoredEvents(aggregateName).ToList()
                                    group storedEvent by storedEvent.StreamId into newGroup
                                    orderby newGroup.Key
                                    select newGroup;

            foreach (var storedEvents in storedEventsGroup)
            {
                var aggregate = (TAggregateRoot)FormatterServices.GetUninitializedObject(typeof(TAggregateRoot));

                foreach (var storedEvent in storedEvents.OrderBy(x => x.Sequence))
                {
                    aggregate.Apply(JsonConvert.DeserializeObject(storedEvent.Data, Type.GetType(storedEvent.DotNetType)));
                }

                aggregates.Add(aggregate);
            }

            return aggregates.AsQueryable();
        }

        public TAggregateRoot Find<TAggregateRoot>(Guid streamId)
            where TAggregateRoot : AggregateRoot
        {
            var aggregate = (TAggregateRoot)FormatterServices.GetUninitializedObject(typeof(TAggregateRoot));

            foreach (var storedEvent in StoredEvents(typeof(TAggregateRoot).Name, new Guid[1] { streamId }).OrderBy(x => x.Sequence))
            {
                aggregate.Apply(JsonConvert.DeserializeObject(storedEvent.Data, Type.GetType(storedEvent.DotNetType)));
            }

            return aggregate;
        }
    }
}
