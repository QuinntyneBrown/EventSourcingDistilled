using BuildingBlocks.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.EventStore
{
    public interface IStreamSetContext
    {
        IQueryable<TAggregateRoot> Set<TAggregateRoot>()
            where TAggregateRoot : AggregateRoot;
    }
    public class StreamSetContext : IStreamSetContext
    {
        private readonly IEventStoreDbContext _dbContext;
        public StreamSetContext(IEventStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<StoredEvent> StoredEvents(string aggregateName,Guid[] streamIds = null, DateTime? limit = null)
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

            var aggregateName = typeof(TAggregateRoot).AssemblyQualifiedName;

            var storedEventsGroup = from storedEvent in StoredEvents(aggregateName)
                    group storedEvent by storedEvent.Aggregate into newGroup
                    orderby newGroup.Key
                    select newGroup;

            foreach(var storedEvents in storedEventsGroup)
            {
                var aggregate = (TAggregateRoot)FormatterServices.GetUninitializedObject(Type.GetType(storedEvents.Key));

                foreach (var storedEvent in storedEvents.OrderBy(x => x.Sequence))
                {
                    aggregate.Apply(storedEvent.Data);
                }

                aggregates.Add(aggregate);
            }

            return aggregates.AsQueryable();
        }

        public TAggregateRoot GetById<TAggregateRoot>(Guid streamId)
            where TAggregateRoot : AggregateRoot
        {
            var aggregate = (AggregateRoot)FormatterServices.GetUninitializedObject(typeof(TAggregateRoot));

            foreach (var storedEvent in _dbContext.StoredEvents.Where(x => x.StreamId == streamId).OrderBy(x => x.Sequence))
            {
                aggregate.Apply(storedEvent.Data);
            }

            return (TAggregateRoot)aggregate;
        }
    }
}
