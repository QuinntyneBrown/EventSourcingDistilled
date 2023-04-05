// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using EventSourcing;
using EventSourcingDistilled.Core.Data;
using EventSourcingDistilled.Core.CustomerAggregateModel;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventSourcingDistilled.Domain.Features;


namespace EventSourcingDistilled.Core.CustomerAggregateModel.Queries;

 public record Request(Guid CustomerId) : IRequest<GetCustomerByIdResponse>;

 public record Response(CustomerDto Customer);

 public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdRequest, GetCustomerByIdResponse>
 {
     private readonly IEventSourcingDistilledDbContext _context;

     public GetCustomerByIdHandler(IEventSourcingDistilledDbContext context) => _context = context;

     public async Task<GetCustomerByIdResponse> Handle(GetCustomerByIdRequest request, CancellationToken cancellationToken)
     {

         var customer = await _context.Customers.FindAsync(request.CustomerId);

         return new(customer.ToDto());
     }
 }

