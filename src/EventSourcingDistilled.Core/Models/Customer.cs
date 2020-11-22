using BuildingBlocks.Abstractions;
using EventSourcingDistilled.Core.DomainEvents.Customer;
using System;

namespace EventSourcingDistilled.Core.Models
{
    public class Customer: AggregateRoot
    {
        public Customer(string firstname, string lastname) 
            => Apply(new CustomerCreated(firstname, lastname));

        protected override void When(dynamic @event) => When(@event);

        private void When(CustomerCreated @event)
        {
            CustomerId = @event.CustomerId;
            Firstname = @event.Firstname;
            Lastname = @event.Lastname;
        }

        private void When(CustomerUpdated @event)
        {
            CustomerId = @event.CustomerId;
            Firstname = @event.Firstname;
            Lastname = @event.Lastname;
        }

        private void When(CustomerRemoved @event)
        {
            IsDeleted = true;
        }

        protected override void EnsureValidState()
        {

        }

        public void Update(string firstname, string lastname, string email, string phoneNumber)
        {
            Apply(new CustomerUpdated(firstname, lastname, email, phoneNumber));
        }

        public void Reomve()
        {
            Apply(new CustomerRemoved());
        }

        public Guid CustomerId { get; private set; }
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public bool IsDeleted { get; private set; }
    }
}
