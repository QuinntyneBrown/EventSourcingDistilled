using BuildingBlocks.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using static Newtonsoft.Json.JsonConvert;

namespace BuildingBlocks.EventStore
{
    public class EventStore : IEventStore
    {
        private readonly ITaskQueue _queue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public Subject<EventStoreChanged> _subject = new Subject<EventStoreChanged>();

        public static ConcurrentDictionary<Guid, DeserializedStoredEvent> Events { get; set; }

        public EventStore(
            ITaskQueue queue = default,
            IServiceScopeFactory serviceScopeFactory = default
            )
        {
            _queue = queue;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<IEnumerable<StoredEvent>> GetEvents()
        {
            var storedEvents = default(IEnumerable<StoredEvent>);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IEventStoreDbContext>();
                storedEvents = context.StoredEvents.OrderBy(x => x.Sequence).ToList();
            }

            return await Task.FromResult(storedEvents);
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
                Add(new StoredEvent()
                {
                    StoredEventId = Guid.NewGuid(),
                    Aggregate = aggregate,
                    AggregateDotNetType = type.AssemblyQualifiedName,
                    Data = SerializeObject(@event),
                    StreamId = aggregateId,
                    DotNetType = @event.GetType().AssemblyQualifiedName,
                    Type = @event.GetType().Name,
                    CreatedOn = System.DateTime.UtcNow,
                    Sequence = Get().Count + 1
                });
            }
            aggregateRoot.ClearChanges();
        }

        public TAggregateRoot Load<TAggregateRoot>(Guid id)
            where TAggregateRoot : AggregateRoot
        {
            var events = Get().Where(x => x.StreamId == id);

            if (!events.Any()) return null;

            var aggregate = (AggregateRoot)FormatterServices.GetUninitializedObject(Type.GetType(typeof(TAggregateRoot).AssemblyQualifiedName));

            foreach (var @event in events)
                aggregate.Apply(@event.Data);

            aggregate.ClearChanges();

            return aggregate as TAggregateRoot;
        }

        public List<DeserializedStoredEvent> Get()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IEventStoreDbContext>();

                if (Events == null)
                    Events = new ConcurrentDictionary<Guid, DeserializedStoredEvent>(context.StoredEvents.Select(x => new DeserializedStoredEvent(x)).ToDictionary(x => x.StoredEventId));

                return Events.Select(x => x.Value)
                    .OrderBy(x => x.CreatedOn)
                    .ToList();
            }
        }

        protected void Add(StoredEvent @event)
        {
            Events.TryAdd(@event.StoredEventId, new DeserializedStoredEvent(@event));

            _subject.OnNext(new EventStoreChanged(@event));

            _queue?.Queue(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<IEventStoreDbContext>();
                    context.StoredEvents.Add(@event);
                    await context.SaveChangesAsync(token);
                }
            });
        }

        public void Subscribe(Action<EventStoreChanged> onNext) => _subject.Subscribe(onNext);
    }
}
