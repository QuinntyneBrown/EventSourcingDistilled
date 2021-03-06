using EventSourcingDistilled.Core.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcingDistilled.Domain.Features
{
    public class GetCustomers
    {
        public record Request : IRequest<Response>;

        public record Response(List<CustomerDto> Customers);

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IEventSourcingDistilledDbContext _context;

            public Handler(IEventSourcingDistilledDbContext context) => _context = context;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
                => new(await _context.Customers.Select(x => x.ToDto()).ToListAsync());
        }
    }
}
