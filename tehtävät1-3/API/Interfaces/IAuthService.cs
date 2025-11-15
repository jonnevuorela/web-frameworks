using API.DTOs;

namespace API.Interfaces
{
    public interface IAuthService
    {
        public Task<LoginRes> Login(LoginReq req);
        public Task<RegisterRes> Register(RegisterReq req);
    }
}
