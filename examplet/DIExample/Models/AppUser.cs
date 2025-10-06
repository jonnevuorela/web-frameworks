
namespace DIExample.Models;

public class AppUser
{
    public long Id { get; set; }
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }
    public required string Username { get; set; }
}
