// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using EventSourcing;
using EventSourcingDistilled.Core.Data;
using EventSourcingDistilled.Core.CustomerAggregateModel;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EventSourcingDistilled.Domain.Features;


namespace EventSourcingDistilled.Core.CustomerAggregateModel.Commands;

 public class CreateCustomerValidator : AbstractValidator<CreateCustomerRequest>
 {
     public CreateCustomerValidator()
     {
         RuleFor(request => request.Customer).NotNull();
         RuleFor(request => request.Customer).SetValidator(new CustomerValidator());
     }
 }

 public record Request(CustomerDto Customer) : IRequest<CreateCustomerResponse>;

 public record Response(CustomerDto Customer);

 public class CreateCustomerHandler : IRequestHandler<CreateCustomerRequest, CreateCustomerResponse>
 {
     private readonly IEventSourcingDistilledDbContext _context;

     public CreateCustomerHandler(EventSourcingDistilledDbContext context)
     {
         _context = context;
     }

     public async Task<CreateCustomerResponse> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
     {

         var customer = new Customer(request.Customer.Firstname, request.Customer.Lastname);

         _context.Add(customer);

         await _context.SaveChangesAsync(cancellationToken);

         return new(customer.ToDto());
     }
 }

