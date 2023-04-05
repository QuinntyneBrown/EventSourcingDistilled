// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;


namespace EventSourcing;

public interface IAggregateRoot
{
    AggregateRoot Apply(IEvent @event);
    void ClearChanges();
    IReadOnlyCollection<IEvent> DomainEvents { get; }
}

