using BuildingBlocks.EventStore;
using EventSourcingDistilled.Core.Data;
using EventSourcingDistilled.Core.Models;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcingDistilled.Domain.Features
{
    public class GetCustomerById
    {
        public record Request(Guid CustomerId) : IRequest<Response>;

        public record Response(CustomerDto Customer);

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IEventSourcingDistilledDbContext _context;

            public Handler(IEventSourcingDistilledDbContext context) => _context = context;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken) {

                var customer = await _context.Customers.FindAsync(request.CustomerId);

                return new (customer.ToDto());
            }
        }
    }
}
