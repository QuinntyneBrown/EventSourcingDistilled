using System;

namespace BuildingBlocks.EventStore
{
    public class MachineDateTime : IDateTime
    {
        public DateTime UtcNow { get { return DateTime.UtcNow; } }
    }
}
