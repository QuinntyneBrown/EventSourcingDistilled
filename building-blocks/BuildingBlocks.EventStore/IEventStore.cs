using BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildingBlocks.EventStore
{
    public interface IEventStore
    {
        void Save(AggregateRoot aggregateRoot);

        TAggregateRoot Load<TAggregateRoot>(Guid id)
            where TAggregateRoot : AggregateRoot;

        void Subscribe(Action<EventStoreChanged> onNext);

        Task<IEnumerable<StoredEvent>> GetEvents();
    }
}
