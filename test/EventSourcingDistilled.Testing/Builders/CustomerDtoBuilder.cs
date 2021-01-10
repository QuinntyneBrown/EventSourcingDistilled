using EventSourcingDistilled.Domain.Features;

namespace EventSourcingDistilled.Testing.Builders
{
    public class CustomerDtoBuilder
    {
        private CustomerDto _customerDto;

        public static CustomerDto WithDefaults()
        {
            return new CustomerDto(default, default, default, default, default);
        }

        public CustomerDtoBuilder()
        {
            _customerDto = WithDefaults();
        }

        public CustomerDto Build()
        {
            return _customerDto;
        }
    }
}
