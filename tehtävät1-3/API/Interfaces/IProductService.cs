using API.DTOs;

namespace API.Interfaces
{
    public interface IProductService
    {
        public Task<ProductRes> Fetch(ProductReq req);
        public Task<ProductRes> Edit(ProductReq req);
        public Task<ProductRes> Add(ProductReq req);
        public Task<Boolean> Remove(ProductReq req);
    }
}
