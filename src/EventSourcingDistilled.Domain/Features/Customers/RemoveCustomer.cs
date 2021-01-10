using BuildingBlocks.EventStore;
using EventSourcingDistilled.Core.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcingDistilled.Domain.Features
{
    public class RemoveCustomer
    {
        public record Request(Guid CustomerId) : IRequest<Unit>;

        public class Handler : IRequestHandler<Request, Unit>
        {
            private readonly IEventStore _context;

            public Handler(IEventStore context) => _context = context;

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken) {

                var customer = await _context.LoadAsync<Customer>(request.CustomerId);
                
                customer.Reomve();

                await _context.SaveChangesAsync(cancellationToken);

                return new();
            }
        }
    }
}
