namespace Domain.Entities
{
    public class TodoItem : BaseEntity
    {
        public string pk => id; // partition key property
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
