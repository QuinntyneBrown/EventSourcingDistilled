using System;

namespace BuildingBlocks.EventStore
{
    public class SnapShot
    {
        public Guid SnapShotId { get; set; }
        public DateTime DateTime { get; set; }
        public string Data { get; set; }
    }
}
