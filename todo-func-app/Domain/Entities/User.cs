namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public string id { get; set; }
        public string pk { get; set; } = "USER";

        // Domain fields
        public string Email { get; set; }
        public string FullName { get; set; }

    }

}
