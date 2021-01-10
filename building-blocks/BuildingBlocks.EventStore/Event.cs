using System;

namespace BuildingBlocks.EventStore
{

    public record Event: IEvent
    {
        public DateTime Created { get; set; } = DateTime.UtcNow;
}
}
