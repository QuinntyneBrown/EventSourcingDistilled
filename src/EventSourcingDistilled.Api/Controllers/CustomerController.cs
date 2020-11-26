using EventSourcingDistilled.Domain.Features.Customers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace EventSourcingDistilled.Api.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController
    {
        private readonly IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(Name = "UpsertCustomerRoute")]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(UpsertCustomer.Response), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<UpsertCustomer.Response>> Update([FromBody] UpsertCustomer.Request request)
            => await _mediator.Send(request);

        [HttpDelete("{customerId}", Name = "RemoveCustomerRoute")]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Unit>> Remove([FromBody] RemoveCustomer.Request request)
            => await _mediator.Send(request);

        [HttpGet(Name = "GetCustomersRoute")]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(GetCustomers.Response), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<GetCustomers.Response>> Get()
            => await _mediator.Send(new GetCustomers.Request());

        [HttpGet("{customerId}",Name = "GetCustomerByIdRoute")]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(GetCustomerById.Response), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<GetCustomerById.Response>> Get([FromQuery]GetCustomerById.Request request)
        {
            var response = await _mediator.Send(request);

            if (response.Customer == null)
            {
                return new NotFoundObjectResult(request.CustomerId);
            }

            return response;
        }
    }
}
