using System.Security.Cryptography;
using API.Models;

namespace API.Services
{
    public class UserService(DataContext _repository) : IUserInterface
    {
        public DataContext Repository { get; } = _repository;

        public async Task<AppUser?> GetByUserName(string username)
        {
            return Repository.Users.FirstOrDefaultAsync(u =>
                u.UserName.ToLower() == username.ToLower()
            );
        }

        public async Tast<AppUser> Register(string username, string password)
        {
            var existingUser = GetByUserName(username);
            if (existingUser != null)
            {
                throw new UserRegistrationException("username in use");
            }
            using var hmac = new HMACSHA512();
            var user = new AppUser { Username = username, Role = "user" };
        }
    }

    [Serializable]
    internal class UserRegistrationException : Exception
    {
        public UserRegistrationException() { }

        public UserRegistrationException(string? message)
            : base(message) { }

        public UserRegistrationException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
