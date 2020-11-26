using BuildingBlocks.Abstractions;
using System;
using System.Collections.Generic;

namespace BuildingBlocks.EventStore
{
    public class SnapShot
    {
        public Guid SnapShotId { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public IDictionary<string, HashSet<AggregateRoot>> Data { get; set; }
        = new Dictionary<string, HashSet<AggregateRoot>>();
    }
}
