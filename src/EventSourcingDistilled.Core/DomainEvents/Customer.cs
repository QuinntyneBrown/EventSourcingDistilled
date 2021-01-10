using BuildingBlocks.EventStore;
using System;

namespace EventSourcingDistilled.Core.DomainEvents
{
    public record CustomerCreated(Guid CustomerId, string Firstname, string Lastname): Event;
    public record CustomerRemoved(DateTime Deleted): Event;
    public record CustomerUpdated(string Firstname, string Lastname, string Email, string PhoneNumber): Event;
}
