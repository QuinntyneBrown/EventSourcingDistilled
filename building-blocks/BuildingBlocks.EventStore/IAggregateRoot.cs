using System.Collections.Generic;

namespace BuildingBlocks.EventStore
{
    public interface IAggregateRoot
    {
        AggregateRoot Apply(object @event);
        void ClearChanges();
        IReadOnlyCollection<object> DomainEvents { get; }
    }
}
