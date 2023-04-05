// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using EventSourcingDistilled.Core.Data;
using EventSourcingDistilled.Domain.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace EventSourcingDistilled.Core.CustomerAggregateModel.Queries;

 public record Request : IRequest<GetCustomersResponse>;

 public record Response(List<CustomerDto> Customers);

 public class GetCustomersHandler : IRequestHandler<GetCustomersRequest, GetCustomersResponse>
 {
     private readonly IEventSourcingDistilledDbContext _context;

     public GetCustomersHandler(IEventSourcingDistilledDbContext context) => _context = context;

     public async Task<GetCustomersResponse> Handle(GetCustomersRequest request, CancellationToken cancellationToken)
         => new(await _context.Customers.Select(x => x.ToDto()).ToListAsync());
 }

