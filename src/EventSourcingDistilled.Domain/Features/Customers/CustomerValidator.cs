using FluentValidation;

namespace EventSourcingDistilled.Domain.Features
{
    public class CustomerValidator : AbstractValidator<CustomerDto>
    {
        public CustomerValidator()
        {

        }
    }
}
