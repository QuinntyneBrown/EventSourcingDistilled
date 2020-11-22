using FluentValidation;
using MediatR;
using EventSourcingDistilled.Core.Data;
using EventSourcingDistilled.Core.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace EventSourcingDistilled.Domain.Features.Customers
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

        public class Request : IRequest<Response> {  
            public CustomerDto Customer { get; set; }
        }

        public class Response
        {
            public CustomerDto Customer { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IEventSourcingDistilledDbContext _context;

            public Handler(IEventSourcingDistilledDbContext context) => _context = context;

            public Task<Response> Handle(Request request, CancellationToken cancellationToken) {

                var customer = new Customer(request.Customer.Firstname, request.Customer.Lastname);

                _context.Save(customer);

                return Task.FromResult(new Response()
                {
                    Customer = customer.ToDto()
                });
            }
        }
    }
}
