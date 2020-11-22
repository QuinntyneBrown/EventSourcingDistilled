using FluentValidation;

namespace EventSourcingDistilled.Domain.Features.Customers
{
    public class CustomerValidator : AbstractValidator<CustomerDto>
    {
        public CustomerValidator()
        {
            
        }
    }
}
