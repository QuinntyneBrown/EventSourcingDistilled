using System;

namespace EventSourcingDistilled.Core.DomainEvents
{
    public class ToDoCreated
    {
        public ToDoCreated(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
