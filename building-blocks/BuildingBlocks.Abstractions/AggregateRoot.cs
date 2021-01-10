using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.Serialization.FormatterServices;

namespace BuildingBlocks.Abstractions
{
    public interface IAggregateRoot
    {
        AggregateRoot Apply(object @event);
        void ClearChanges();
        IReadOnlyCollection<object> DomainEvents { get; }
    }
    public abstract class AggregateRoot: IAggregateRoot
    {
        internal List<object> _events = new List<object>();

        [NotMapped]
        public IReadOnlyCollection<object> DomainEvents => _events.AsReadOnly();

        public AggregateRoot(IEnumerable<object> events)
        {
            foreach (var @event in events) { When(@event); }
        }

        protected AggregateRoot()
        {

        }

        public void RaiseDomainEvent(object @event)
        {
            _events ??= new List<object>();
            _events.Add(@event);
        }
        public void ClearChanges() => _events?.Clear();
        public AggregateRoot Apply(object @event)
        {
            When(@event);
            EnsureValidState();
            RaiseDomainEvent(@event);
            return this;
        }
        protected abstract void When(dynamic @event);
        protected abstract void EnsureValidState();

        public static TAggregateRoot Create<TAggregateRoot>()
            => (TAggregateRoot)GetUninitializedObject(typeof(TAggregateRoot));

    }
}
