using System;

namespace EventSourcingDistilled.Core.DomainEvents.Customer
{
    public class CustomerUpdated
    {
        public CustomerUpdated(string firstname, string lastname, string email, string phoneNumber)
        {
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public Guid CustomerId { get; set; } = Guid.NewGuid();
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
    }
}
