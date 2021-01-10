using System;

namespace EventSourcingDistilled.Domain.Features
{
    public record CustomerDto(Guid CustomerId, string Firstname, string Lastname, string Email, string PhoneNumber);
}
