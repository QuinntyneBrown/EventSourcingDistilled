// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Http;
using System;


namespace EventSourcing;

public class CorrelationIdAccessor : ICorrelationIdAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdAccessor(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public Guid CorrelationId => _httpContextAccessor != null && _httpContextAccessor.HttpContext != null
            ? new Guid(_httpContextAccessor.HttpContext.Request.Headers["correlationId"])
            : Guid.NewGuid();
}

