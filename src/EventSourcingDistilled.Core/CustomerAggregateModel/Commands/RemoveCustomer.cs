// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using EventSourcing;
using EventSourcing;
using EventSourcingDistilled.Core.Data;
using EventSourcingDistilled.Core.CustomerAggregateModel;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace EventSourcingDistilled.Core.CustomerAggregateModel.Commands;

 public record Request(Guid CustomerId) : IRequest<Unit>;

 public class RemoveCustomerHandler : IRequestHandler<Request, Unit>
 {
     private readonly IEventSourcingDistilledDbContext _context;
     private readonly IDateTime _dateTime;

     public RemoveCustomerHandler(IDateTime dateTime, IEventSourcingDistilledDbContext context)
     {
         _context = context;
         _dateTime = dateTime;
     }

     public async Task<Unit> Handle(RemoveCustomerRequest request, CancellationToken cancellationToken)
     {

         var customer = await _context.LoadAsync<Customer>(request.CustomerId);

         customer.Reomve(_dateTime.UtcNow);

         await _context.SaveChangesAsync(cancellationToken);

         return new();
     }
 }

