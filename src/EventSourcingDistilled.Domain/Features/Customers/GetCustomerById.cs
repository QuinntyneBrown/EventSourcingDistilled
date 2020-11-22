using BuildingBlocks.Abstractions;
using EventSourcingDistilled.Core.Models;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            private readonly IAppDbContext _context;

            public Handler(IAppDbContext context) => _context = context;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken) {

                var query = from customer in _context.Set<Customer>()
                        where customer.CustomerId == request.CustomerId
                        select customer.ToDto();

                return new Response() { 
                    Customer = query.SingleOrDefault()
                };
            }
        }
    }
}
