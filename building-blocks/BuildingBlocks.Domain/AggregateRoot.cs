using System.Collections.Generic;

namespace BuildingBlocks.Domain
{
    public abstract class AggregateRoot
    {
        private List<object> _events = new List<object>();
        public IReadOnlyCollection<object> DomainEvents => _events.AsReadOnly();
        public void RaiseDomainEvent(object @event)
        {
            _events ??= new List<object>();
            _events.Add(@event);
        }
        public void ClearChanges() => _events.Clear();
        public void Apply(object @event)
        {
            When(@event);
            EnsureValidState();
            RaiseDomainEvent(@event);
        }
        protected abstract void When(object @event);
        protected abstract void EnsureValidState();
    }

}