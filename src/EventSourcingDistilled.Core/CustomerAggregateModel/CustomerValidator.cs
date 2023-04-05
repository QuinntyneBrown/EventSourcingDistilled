// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using FluentValidation;


namespace EventSourcingDistilled.Domain.Features;

public class CustomerValidator : AbstractValidator<CustomerDto>
{
    public CustomerValidator()
    {

    }
}

