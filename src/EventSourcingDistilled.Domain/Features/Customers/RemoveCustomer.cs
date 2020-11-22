using BuildingBlocks.Abstractions;
using EventSourcingDistilled.Core.Models;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcingDistilled.Domain.Features.Customers
{
    public class RemoveCustomer
    {
        public class Request : IRequest<Unit> {  
            public Guid CustomerId { get; set; }        
        }

        public class Handler : IRequestHandler<Request, Unit>
        {
            private readonly IAppDbContext _context;

            public Handler(IAppDbContext context) => _context = context;

            public Task<Unit> Handle(Request request, CancellationToken cancellationToken) {

                var customer = _context.Set<Customer>().First(x => x.CustomerId == request.CustomerId);

                customer.Reomve();

                _context.Store(customer);

                return Task.FromResult(new Unit());
            }
        }
    }
}
