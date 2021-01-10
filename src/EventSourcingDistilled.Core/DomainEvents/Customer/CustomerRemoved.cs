using System;

namespace EventSourcingDistilled.Core.DomainEvents.Customer
{
    public class CustomerRemoved
    {        
        public CustomerRemoved()
        {

        }

        public DateTime Deleted { get; set; }
    }
}
