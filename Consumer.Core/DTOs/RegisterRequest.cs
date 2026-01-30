namespace ConsumerEnpoints.Models
{
    public class RegisterRequest
    {
        public string ConsumerName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
