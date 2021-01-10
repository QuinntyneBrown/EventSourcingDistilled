using BuildingBlocks.Abstractions;
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
            private readonly IAppDbContext _context;

            public Handler(IAppDbContext context) => _context = context;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken) {

                var customer = await _context.FindAsync<Customer>(request.CustomerId);

                return new (customer.ToDto());
            }
        }
    }
}
