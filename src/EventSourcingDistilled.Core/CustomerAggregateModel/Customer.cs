// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using EventSourcing;
using EventSourcingDistilled.Core.DomainEvents;
using Microsoft.EntityFrameworkCore;
using System;


namespace EventSourcingDistilled.Core.Models;

[Index("Email")]
public class Customer : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public string Firstname { get; private set; }
    public string Lastname { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public DateTime? Deleted { get; private set; }

    public Customer(string firstname, string lastname)
        => Apply(new CustomerCreated(Guid.NewGuid(), firstname, lastname));

    protected override void When(dynamic @event) => When(@event);

    private void When(CustomerCreated customerCreated)
    {
        CustomerId = customerCreated.CustomerId;
        Firstname = customerCreated.Firstname;
        Lastname = customerCreated.Lastname;
    }

    private void When(CustomerUpdated @event)
    {
        Firstname = @event.Firstname;
        Lastname = @event.Lastname;
    }

    private void When(CustomerRemoved customerRemoved)
    {
        Deleted = customerRemoved.Deleted;
    }

    protected override void EnsureValidState()
    {

    }

    public void Update(string firstname, string lastname, string email, string phoneNumber)
    {
        Apply(new CustomerUpdated(firstname, lastname, email, phoneNumber));
    }

    public void Reomve(DateTime deleted)
    {
        Apply(new CustomerRemoved(deleted));
    }
}

