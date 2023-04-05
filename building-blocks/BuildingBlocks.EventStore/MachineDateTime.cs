// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;


namespace EventSourcing;

public class MachineDateTime : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
}

