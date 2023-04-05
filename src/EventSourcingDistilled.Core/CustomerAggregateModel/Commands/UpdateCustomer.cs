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

 public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerRequest>
 {
     public UpdateCustomerValidator()
     {
         RuleFor(request => request.Customer).NotNull();
         RuleFor(request => request.Customer).SetValidator(new CustomerValidator());
     }
 }

 public record Request(CustomerDto Customer) : IRequest<UpdateCustomerResponse>;

 public record Response(CustomerDto Customer);

 public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerRequest, UpdateCustomerResponse>
 {
     private readonly IEventSourcingDistilledDbContext _context;
     private readonly IDateTime _dateTime;

     public UpdateCustomerHandler(IDateTime dateTime, IEventSourcingDistilledDbContext context)
     {
         _context = context;
         _dateTime = dateTime;
     }

     public async Task<UpdateCustomerResponse> Handle(UpdateCustomerRequest request, CancellationToken cancellationToken)
     {

         var customer = await _context.LoadAsync<Customer>(request.Customer.CustomerId);

         customer.Update(
             request.Customer.Firstname,
             request.Customer.Lastname,
             request.Customer.Email,
             request.Customer.PhoneNumber);

         await _context.SaveChangesAsync(cancellationToken);

         return new(customer.ToDto());
     }
 }

