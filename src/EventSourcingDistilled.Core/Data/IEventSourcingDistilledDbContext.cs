// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using EventSourcing;
using EventSourcingDistilled.Core.CustomerAggregateModel;
using Microsoft.EntityFrameworkCore;


namespace EventSourcingDistilled.Core.Data;

public interface IEventSourcingDistilledDbContext : IEventStore
{
    DbSet<Customer> Customers { get; }
}

