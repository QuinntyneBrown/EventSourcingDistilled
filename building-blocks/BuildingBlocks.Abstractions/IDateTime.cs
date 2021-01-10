using System;

namespace BuildingBlocks.Abstractions
{
    public interface IDateTime
    {
        System.DateTime UtcNow { get; }
    }

    public class MachineDateTime : IDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
