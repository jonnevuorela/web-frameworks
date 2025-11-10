using API.Models;

namespace API.Interfaces
{
    public interface IProductsRepository : IDisposable
    {
        public Task<IEnumerable<AppProduct>> GetAll();
        public Task<AppProduct?> GetById(long id);

        public Task<AppProduct?> Save(string name, long? id = null);

        public Task<bool> Remove(long id);
    }
}
