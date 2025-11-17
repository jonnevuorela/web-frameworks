using API.Models;

namespace API.Interfaces
{
    public interface IUserService
    {
        Task<AppUser?> GetByUserName(string username);
        Task<AppUser> Register(string username, string password);

        Task<string> Login(string username, string password);

        Task<IEnumerable<AppUser>> GetAll();

        Task<AppUser> GetAccount(int id);
    }
}
