using FluentValidation;
using MediatR;
using EventSourcingDistilled.Core.Data;
using EventSourcingDistilled.Core.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace EventSourcingDistilled.Domain.Features.Customers
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

                var customer = _context.Set<Customer>().FirstOrDefault(x => x.CustomerId == request.Customer.CustomerId);

                customer.Update(
                    request.Customer.Firstname,
                    request.Customer.Lastname,
                    request.Customer.Email,
                    request.Customer.PhoneNumber);

                _context.Save(customer);

                return Task.FromResult(new Response()
                {
                    Customer = customer.ToDto()
                });
            }
        }
    }
}
