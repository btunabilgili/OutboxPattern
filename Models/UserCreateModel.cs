namespace OutboxPattern.Models
{
    public class UserCreateModel
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
    }
}
