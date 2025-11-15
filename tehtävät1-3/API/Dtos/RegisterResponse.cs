namespace API.DTOs
{
    public class RegisterRes
    {
        public required long Id { get; set; }

        public required string Firstname { get; set; }

        public required string Lastname { get; set; }
        public required string Username { get; set; }
    }
}
