namespace BuildingBlocks.Scheduling.Hangfire
{
    public class HangfireOptions
    {
        public string ConnectionString { get; set; }
        public bool UseInMemoryStorage { get; set; }
    }
}
