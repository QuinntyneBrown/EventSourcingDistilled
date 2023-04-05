// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;


namespace EventSourcingDistilled.Domain.Features;

public record CustomerDto(Guid CustomerId, string FirstName, string LastName, string Email, string PhoneNumber);

