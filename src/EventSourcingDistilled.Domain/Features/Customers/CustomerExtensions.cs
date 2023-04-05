using EventSourcingDistilled.Core.CustomerAggregateModel;
using EventSourcingDistilled.Domain.Features;

namespace EventSourcingDistilled.Domain.Features
{
    public static class CustomerExtensions
    {
        public static CustomerDto ToDto(this Customer customer)
            => new(customer.CustomerId, customer.Firstname, customer.Lastname, customer.Email, customer.PhoneNumber);
    }
}
