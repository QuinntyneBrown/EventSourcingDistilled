using EventSourcingDistilled.Core.Models;
using EventSourcingDistilled.Domain.Features;

namespace EventSourcingDistilled.Domain.Features
{
    public static class CustomerExtensions
    {
        public static CustomerDto ToDto(this Customer customer)
        {
            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                Firstname = customer.Firstname,
                Lastname = customer.Lastname,
            };
        }
    }
}
