using BuildingBlocks.EventStore;
using EventSourcingDistilled.Domain.Features.Customers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcingDistilled.Api.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController
    {
        private readonly IMediator _mediator;
        private readonly IEventStore _eventStore;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CustomersController(IMediator mediator, IEventStore eventStore, IHttpContextAccessor httpContextAccessor)
        {
            _eventStore = eventStore;
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }

        [HttpPost(Name = "CreateCustomerRoute")]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CreateCustomer.Response), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CreateCustomer.Response>> Create([FromBody] CreateCustomer.Request request)
            => await _mediator.Send(request);

        [HttpPut(Name = "UpdateCustomerRoute")]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(UpdateCustomer.Response), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<UpdateCustomer.Response>> Update([FromBody] UpdateCustomer.Request request)
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

        [HttpGet("ss")]
        public async Task ServerSentEvents(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            var response = _httpContextAccessor.HttpContext.Response;

            response.Headers.Add("Content-Type", "text/event-stream");

            _eventStore.Subscribe(async e =>
            {
                if (e.Event.Aggregate == "Customer")
                {
                    await response
                    .WriteAsync($"data: {JsonConvert.SerializeObject(e)}\r\r");

                    response.Body.Flush();
                }

            });

            await tcs.Task;

        }
    }
}
