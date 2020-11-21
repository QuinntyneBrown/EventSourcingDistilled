using System;

namespace BuildingBlocks.Domain
{
    public class AggregateRootCreated
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
