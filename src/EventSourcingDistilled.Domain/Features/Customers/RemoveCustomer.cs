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
            private readonly IEventStore _store;

            public Handler(IEventStore store) => _store = store;

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken) {

                var customer = await _store.LoadAsync<Customer>(request.CustomerId);
                
                customer.Reomve();

                await _store.SaveChangesAsync(cancellationToken);

                return new();
            }
        }
    }
}
