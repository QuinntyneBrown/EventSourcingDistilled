using EventSourcingDistilled.Core.Common;
using EventSourcingDistilled.Core.DomainEvents;

namespace EventSourcingDistilled.Core.Models
{
    public class ToDo: AggregateRoot
    {
        public ToDo()
        {

        }

        public ToDo(string name) 
            => Apply(new ToDoCreated(name));

        protected override void When(object @event)
        {
            if(@event is ToDoCreated toDoCreated)
            {
                Name = toDoCreated.Name;
            }
        }

        protected override void EnsureValidState()
        {

        }

        public string Name { get; set; }

    }
}
