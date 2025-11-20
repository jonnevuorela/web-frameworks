using API.Dtos;
using API.Interfaces;

namespace API.Services
{
    public class AuthService(IAuthRepo _authRepo, ILogRepo _logRepo) : IAuthService
    {
        public async Task<LoginRes?> Login(LoginReq req)
        {
            var user = await _authRepo.Login(req.UserName, req.Password);
            if (user != null)
            {
                await _logRepo.Create(new AddLogEntryReq { UserName = user.Username });

                return new LoginRes { Token = "jwt" };
            }

            return null;
        }

        public async Task<RegisterRes?> Register(RegisterReq req)
        {
            var user = await _authRepo.Create(req);
            if (user != null && user.Id != null)
            {
                return new RegisterRes
                {
                    Id = (long)user.Id,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Username = user.Username,
                };
            }

            return null;
        }
    }
}
