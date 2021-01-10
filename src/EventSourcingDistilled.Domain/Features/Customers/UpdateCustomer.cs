using BuildingBlocks.EventStore;
using EventSourcingDistilled.Core.Models;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcingDistilled.Domain.Features
{
    public class UpdateCustomer
    {
        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(request => request.Customer).NotNull();
                RuleFor(request => request.Customer).SetValidator(new CustomerValidator());
            }
        }

        public record Request(CustomerDto Customer): IRequest<Response>;

        public record Response(CustomerDto Customer);

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IEventStore _store;

            public Handler(IEventStore store) => _store = store;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {

                var customer = await _store.LoadAsync<Customer>(request.Customer.CustomerId);

                customer.Update(
                    request.Customer.Firstname,
                    request.Customer.Lastname,
                    request.Customer.Email,
                    request.Customer.PhoneNumber);

                await _store.SaveChangesAsync(cancellationToken);

                return new(customer.ToDto());
            }
        }
    }
}
