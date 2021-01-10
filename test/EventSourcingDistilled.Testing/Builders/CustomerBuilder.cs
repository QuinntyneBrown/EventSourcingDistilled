using EventSourcingDistilled.Core.Models;

namespace EventSourcingDistilled.Testing.Builders
{
    public class CustomerBuilder
    {
        private Customer _customer;

        public static Customer WithDefaults()
        {
            return new Customer("Quinn", "Brown");
        }

        public CustomerBuilder()
        {
            _customer = WithDefaults();
        }

        public Customer Build()
        {
            return _customer;
        }
    }
}
