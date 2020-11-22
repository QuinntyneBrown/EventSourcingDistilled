using MediatR;
using EventSourcingDistilled.Core.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcingDistilled.Core.Models;
using System.Linq;

namespace EventSourcingDistilled.Domain.Features.Customers
{
    public class GetCustomerById
    {
        public class Request : IRequest<Response> {  
            public Guid CustomerId { get; set; }        
        }

        public class Response
        {
            public CustomerDto Customer { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IEventSourcingDistilledDbContext _context;

            public Handler(IEventSourcingDistilledDbContext context) => _context = context;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken) {

                var customer = _context.Set<Customer>().First(x => x.CustomerId == request.CustomerId);

                return new Response() { 
                    Customer = customer.ToDto()
                };
            }
        }
    }
}
