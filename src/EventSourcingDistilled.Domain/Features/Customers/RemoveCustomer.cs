using EventSourcing;
using EventSourcing;
using EventSourcingDistilled.Core.Data;
using EventSourcingDistilled.Core.CustomerAggregateModel;
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
            private readonly IEventSourcingDistilledDbContext _context;
            private readonly IDateTime _dateTime;

            public Handler(IDateTime dateTime, IEventSourcingDistilledDbContext context)
            {
                _context = context;
                _dateTime = dateTime;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {

                var customer = await _context.LoadAsync<Customer>(request.CustomerId);

                customer.Reomve(_dateTime.UtcNow);

                await _context.SaveChangesAsync(cancellationToken);

                return new();
            }
        }
    }
}
