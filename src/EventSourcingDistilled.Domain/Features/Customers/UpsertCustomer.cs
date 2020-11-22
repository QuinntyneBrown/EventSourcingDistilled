using FluentValidation;
using MediatR;
using BuildingBlocks.Abstractions;
using EventSourcingDistilled.Core.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace EventSourcingDistilled.Domain.Features.Customers
{
    public class UpsertCustomer
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
            private readonly IAppDbContext _context;

            public Handler(IAppDbContext context) => _context = context;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken) {

                var customer = _context.Set<Customer>().FirstOrDefault(x => x.CustomerId == request.Customer.CustomerId);

                if (customer == null)
                {
                    customer = new Customer(request.Customer.Firstname, request.Customer.Lastname);
                }
                else
                {
                    customer.Update(
                        request.Customer.Firstname,
                        request.Customer.Lastname,
                        request.Customer.Email,
                        request.Customer.PhoneNumber);
                }

                _context.Store(customer);

                await _context.SaveChangesAsync(cancellationToken);

                return new Response()
                {
                    Customer = customer.ToDto()
                };
            }
        }
    }
}
