using Microsoft.AspNetCore.Http;
using System;

namespace BuildingBlocks.EventStore
{
    public class CorrelationIdAccessor : ICorrelationIdAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationIdAccessor(IHttpContextAccessor httpContextAccessor)
            => _httpContextAccessor = httpContextAccessor;

        public Guid CorrelationId => _httpContextAccessor != null && _httpContextAccessor.HttpContext != null
                ? new Guid(_httpContextAccessor.HttpContext.Request.Headers["correlationId"])
                : Guid.NewGuid();
    }
}
