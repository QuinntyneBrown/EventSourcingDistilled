using BuildingBlocks.Domain;
using EventSourcingDistilled.Core.DomainEvents;
using System;

namespace EventSourcingDistilled.Core.Models
{
    public class ToDo: AggregateRoot
    {
        public ToDo(string name) 
            => Apply(new ToDoCreated(name));

        protected override void When(object @event)
        {
            if(@event is ToDoCreated toDoCreated)
            {
                ToDoId = toDoCreated.ToDoId;
                Name = toDoCreated.Name;                
            }
        }

        protected override void EnsureValidState()
        {

        }

        public Guid ToDoId { get; set; }
        public string Name { get; set; }

    }
}
