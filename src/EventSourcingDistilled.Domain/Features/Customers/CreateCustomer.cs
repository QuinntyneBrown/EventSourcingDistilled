using BuildingBlocks.EventStore;
using EventSourcingDistilled.Core.Data;
using EventSourcingDistilled.Core.Models;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcingDistilled.Domain.Features
{
    public class CreateCustomer
    {
        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(request => request.Customer).NotNull();
                RuleFor(request => request.Customer).SetValidator(new CustomerValidator());
            }
        }

        public record Request(CustomerDto Customer) : IRequest<Response>;

        public record Response(CustomerDto Customer);

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IEventSourcingDistilledDbContext _context;

            public Handler(EventSourcingDistilledDbContext context)
            {
                _context = context;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {

                var customer = new Customer(request.Customer.Firstname, request.Customer.Lastname);

                _context.Add(customer);

                await _context.SaveChangesAsync(cancellationToken);

                return new(customer.ToDto());
            }
        }
    }
}
