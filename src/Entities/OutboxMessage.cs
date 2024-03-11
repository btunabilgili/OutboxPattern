namespace OutboxPattern.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public required string Payload { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsProcessed { get; set; }
    }
}
