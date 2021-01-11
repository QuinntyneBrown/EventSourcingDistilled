using BuildingBlocks.EventStore;
using EventSourcingDistilled.Core.Data;
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
            private readonly IEventSourcingDistilledDbContext _context;
            private readonly IDateTime _dateTime;

            public Handler(IDateTime dateTime, IEventSourcingDistilledDbContext context)
            {
                _context = context;
                _dateTime = dateTime;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {

                var customer = await _context.LoadAsync<Customer>(request.Customer.CustomerId);

                customer.Update(
                    request.Customer.Firstname,
                    request.Customer.Lastname,
                    request.Customer.Email,
                    request.Customer.PhoneNumber);

                await _context.SaveChangesAsync(cancellationToken);

                return new(customer.ToDto());
            }
        }
    }
}
