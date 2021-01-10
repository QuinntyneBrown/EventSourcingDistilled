using System;

namespace EventSourcingDistilled.Core.DomainEvents
{
    public record CustomerCreated(Guid CustomerId, string Firstname, string Lastname);
    public record CustomerRemoved(DateTime Deleted);
    public record CustomerUpdated(string Firstname, string Lastname, string Email, string PhoneNumber);
}
