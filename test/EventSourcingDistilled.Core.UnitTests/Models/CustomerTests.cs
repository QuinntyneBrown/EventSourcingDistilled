using EventSourcingDistilled.Core.CustomerAggregateModel;
using Xunit;

namespace EventSourcingDistilled.Core.UnitTests.Models
{
    public class CustomerTests
    {
        [Fact]
        public void ShouldHandleCustomerCreatedEvent()
        {
            var firstname = "Quinntyne";
            var lastname = "Brown";

            var customer = new Customer(firstname, lastname);

            Assert.Equal(firstname, customer.Firstname);

            Assert.Single(customer.DomainEvents);
        }
    }
}
