namespace API.Models
{
    public class AppLog
    {
        public required long Id { get; set; }
        public required string Timestamp { get; set; }
        public required string UserName { get; set; }
    }
}
