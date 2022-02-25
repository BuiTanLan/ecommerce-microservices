namespace BuildingBlocks.Abstractions.Persistence.EventStore;

public class Stream
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public int Version { get; set; }
    public IList<StreamEvent> Events { get; set; } = new List<StreamEvent>();
}
