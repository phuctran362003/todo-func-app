namespace Domain.Entities
{
    public class TodoItem : BaseEntity
    {
        // Cosmos required fields
        public string? userId { get; set; }           // Partition Key
        public string pk => userId ?? "system";      // PartitionKey binding property with default

        // Domain fields
        public required string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
