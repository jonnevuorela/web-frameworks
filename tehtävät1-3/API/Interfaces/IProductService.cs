using API.Dtos;

namespace API.Interfaces
{
    public interface IProductService
    {
        public Task<IEnumerable<ProductRes?>> FetchAll();
        public Task<ProductRes?> Fetch(long id);
        public Task<ProductRes?> Edit(long id, ProductReq req);
        public Task<ProductRes?> Add(ProductReq req);
        public Task<Boolean> Remove(long id);
    }
}
