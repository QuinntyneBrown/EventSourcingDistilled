using System;

namespace EventSourcingDistilled.Core.DomainEvents.Customer
{
    public class CustomerCreated
    {
        public CustomerCreated(Guid customerId, string firstname, string lastname)
        {
            CustomerId = customerId;
            Firstname = firstname;
            Lastname = lastname;
        }

        public Guid CustomerId { get; set; }
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
    }
}
