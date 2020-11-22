using System;

namespace EventSourcingDistilled.Core.DomainEvents.Customer
{
    public class CustomerCreated
    {
        public CustomerCreated(string firstname, string lastname)
        {
            Firstname = firstname;
            Lastname = lastname;
        }

        public Guid CustomerId { get; set; } = Guid.NewGuid();
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
    }
}
