using BuildingBlocks.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BuildingBlocks.EventStore
{
    public interface IRepository
    {
        TAggregateRoot[] Query<TAggregateRoot>() where TAggregateRoot : AggregateRoot;

        TAggregateRoot Query<TAggregateRoot>(Guid id) where TAggregateRoot : AggregateRoot;

        TAggregateRoot[] Query<TAggregateRoot>(IEnumerable<Guid> ids) where TAggregateRoot : AggregateRoot;

        void OnNext(EventStoreChanged onNext);
    }

    public class Repository : IRepository
    {
        private ConcurrentDictionary<string, ConcurrentBag<AggregateRoot>> _aggregates { get; set; }
            = new ConcurrentDictionary<string, ConcurrentBag<AggregateRoot>>();

        public Repository(IEventStore eventStore)
        {
            var events = eventStore.GetEvents().GetAwaiter().GetResult();

            if (events != null)
                foreach (var @event in events)
                    OnNext(new EventStoreChanged(@event));

            eventStore.Subscribe(OnNext);
        }

        public void OnNext(EventStoreChanged value)
        {
            var type = Type.GetType(value.Event.AggregateDotNetType);
            var aggregates = _aggregates.SingleOrDefault(x => x.Key == value.Event.AggregateDotNetType).Value;
            var aggregateId = value.Event.StreamId;

            if (aggregates != null)
            {
                AggregateRoot e = default;
                foreach (var aggregate in aggregates)
                {
                    if (value.Event.StreamId == (Guid)type.GetProperty($"{type.Name}Id").GetValue(aggregate, null))
                        e = aggregate;
                }

                if (e == default)
                    e = (AggregateRoot)FormatterServices.GetUninitializedObject(Type.GetType(value.Event.AggregateDotNetType));


                e.Apply(JsonConvert.DeserializeObject(value.Event.Data, Type.GetType(value.Event.DotNetType)));

                e.ClearChanges();

                var newAggregates = new ConcurrentBag<AggregateRoot>() { e };

                foreach (var originalAggregate in aggregates)
                {
                    var originalId = (Guid)type.GetProperty($"{type.Name}Id").GetValue(originalAggregate, null);

                    if (aggregateId != originalId)
                        newAggregates.Add(originalAggregate);
                }

                _aggregates.TryUpdate(type.AssemblyQualifiedName, newAggregates, aggregates);
            }

            if (aggregates == null)
            {
                var aggregate = (AggregateRoot)FormatterServices.GetUninitializedObject(Type.GetType(value.Event.AggregateDotNetType));

                aggregates = new ConcurrentBag<AggregateRoot>() { aggregate };

                var domainEvent = JsonConvert.DeserializeObject(value.Event.Data, Type.GetType(value.Event.DotNetType));

                aggregate.Apply(domainEvent);

                aggregate.ClearChanges();

                _aggregates.TryAdd(value.Event.AggregateDotNetType, aggregates);

            }
        }

        public TAggregateRoot[] Query<TAggregateRoot>() where TAggregateRoot : AggregateRoot
        {
            var result = new List<TAggregateRoot>();
            var assemblyQualifiedName = typeof(TAggregateRoot).AssemblyQualifiedName;

            _aggregates.TryGetValue(assemblyQualifiedName, out ConcurrentBag<AggregateRoot> aggregates);

            if (aggregates != null)
                foreach (var a in aggregates)
                    result.Add(a as TAggregateRoot);

            return result.ToArray();
        }

        public TAggregateRoot[] Query<TAggregateRoot>(IEnumerable<Guid> ids) where TAggregateRoot : AggregateRoot
        {
            var type = typeof(TAggregateRoot);
            var result = new List<TAggregateRoot>();
            var assemblyQualifiedName = typeof(TAggregateRoot).AssemblyQualifiedName;

            _aggregates.TryGetValue(assemblyQualifiedName, out ConcurrentBag<AggregateRoot> aggregates);

            if (aggregates == null) return result.ToArray();

            foreach (var aggregate in aggregates)
            {
                if (ids.Contains((Guid)type.GetProperty($"{type.Name}Id").GetValue(aggregate, null)))
                    result.Add(aggregate as TAggregateRoot);
            }

            return result.ToArray();
        }

        public TAggregateRoot Query<TAggregateRoot>(Guid id) where TAggregateRoot : AggregateRoot
        {
            var type = typeof(TAggregateRoot);
            var result = default(TAggregateRoot);

            foreach (var aggregate in Query<TAggregateRoot>())
            {
                if (id == (Guid)type.GetProperty($"{type.Name}Id").GetValue(aggregate, null))
                    result = aggregate;
            }

            return result;
        }
    }
}
