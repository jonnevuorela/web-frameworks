using API.Models;

namespace API.Interfaces
{
    public interface IUserService
    {
        public Task<AppUser?> GetByUserName(string username);
        public Task<AppUser> Register(string username, string password);

        Task<string> Login(string username, string password);

        public Task<IEnumerable<AppUser>> GetAll();

        public Task<AppUser> GetAccount(int id);
    }
}
