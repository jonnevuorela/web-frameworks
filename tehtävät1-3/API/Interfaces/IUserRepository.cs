using API.Models;

namespace API.Interfaces
{
    // huomaa, että IUsersRepository-interfacemme perii IDisposable-interfacen
    public interface IUsersRepository : IDisposable
    {
        public Task<IEnumerable<AppUser>> GetAll();
        public Task<AppUser?> GetById(long id);

        public Task<AppUser?> Save(
            string firstname,
            string lastname,
            string username,
            long? id = null
        );

        public Task<bool> Remove(long id);
    }
}
