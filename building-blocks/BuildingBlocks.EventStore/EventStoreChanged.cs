namespace BuildingBlocks.EventStore
{
    public class EventStoreChanged
    {
        public EventStoreChanged(StoredEvent @event)
        {
            Event = @event;
        }

        public StoredEvent Event { get; set; }
    }
}
