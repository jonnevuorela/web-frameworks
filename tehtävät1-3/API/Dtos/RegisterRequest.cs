namespace API.Dtos
{
    public class RegisterReq
    {
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Password { get; set; }

        public required string UserName { get; set; }
    }
}
