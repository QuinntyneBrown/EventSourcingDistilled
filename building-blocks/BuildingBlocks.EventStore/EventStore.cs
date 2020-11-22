using BuildingBlocks.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Newtonsoft.Json.JsonConvert;

namespace BuildingBlocks.EventStore
{
    public class EventStore : IEventStore
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private List<StoredEvent> storedEvents = new List<StoredEvent>();
        public EventStore(
            IServiceScopeFactory serviceScopeFactory = default
            )
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Save(AggregateRoot aggregateRoot)
        {
            var type = aggregateRoot.GetType();
            Guid aggregateId = (Guid)type.GetProperty($"{type.Name}Id").GetValue(aggregateRoot, null);
            string aggregate = aggregateRoot.GetType().Name;

            if(aggregateRoot.DomainEvents == null || aggregateRoot.DomainEvents.Count == 0)
            {
                aggregateRoot.Apply(new AggregateRootCreated());
            }

            foreach (var @event in aggregateRoot.DomainEvents)
            {
                storedEvents.Add(new StoredEvent()
                {
                    StoredEventId = Guid.NewGuid(),
                    Aggregate = aggregate,
                    AggregateDotNetType = type.AssemblyQualifiedName,
                    Data = SerializeObject(@event),
                    StreamId = aggregateId,
                    DotNetType = @event.GetType().AssemblyQualifiedName,
                    Type = @event.GetType().Name,
                    CreatedOn = System.DateTime.UtcNow,
                    Sequence = 0
                });
            }
            aggregateRoot.ClearChanges();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IEventStoreDbContext>();

                foreach(var e in storedEvents)
                {
                    context.StoredEvents.Add(e);
                }
                
                var result =  await context.SaveChangesAsync(cancellationToken);

                storedEvents.Clear();

                return result;
            }
        }

    }
}
