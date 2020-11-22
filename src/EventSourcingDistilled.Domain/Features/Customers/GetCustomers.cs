using BuildingBlocks.Abstractions;
using EventSourcingDistilled.Core.Models;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcingDistilled.Domain.Features.Customers
{
    public class GetCustomers
    {
        public class Request : IRequest<Response> {  }

        public class Response
        {
            public List<CustomerDto> Customers { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IAppDbContext _context;

            public Handler(IAppDbContext context) => _context = context;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken) {
			    return new Response() { 
                    Customers = _context.Set<Customer>().Select(x => x.ToDto()).ToList()
                };
            }
        }
    }
}
