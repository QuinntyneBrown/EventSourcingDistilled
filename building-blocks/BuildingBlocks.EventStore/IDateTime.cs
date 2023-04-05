// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.


namespace EventSourcing;

public interface IDateTime
{
    System.DateTime UtcNow { get; }
}

