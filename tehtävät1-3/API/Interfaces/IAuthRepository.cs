using API.DTOs;
using API.Models;

namespace API.Interfaces
{
    public interface IAuthRepo
    {
        public Task<AppUser?> Create(RegisterReq req);
        public Task<AppUser?> Login(string username, string password);
    }
}
