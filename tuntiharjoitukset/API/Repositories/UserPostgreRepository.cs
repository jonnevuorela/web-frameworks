using API.Interfaces;
using API.Models;

namespace API.Repositories
{
    public class UserPostgreRepository : IUsersRepository
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppUser>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<AppUser?> GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Remove(long id)
        {
            throw new NotImplementedException();
        }

        public Task<AppUser?> Save(
            string firstname,
            string lastname,
            string username,
            long? id = null
        )
        {
            throw new NotImplementedException();
        }
    }
}
