using API.Dtos;
using API.Interfaces;

namespace API.Services
{
    public class ProductService(IProductsRepo _productsRepo) : IProductService
    {
        public async Task<IEnumerable<ProductRes?>> FetchAll()
        {
            var products = (await _productsRepo.GetAll()).ToList();
            var responses = new List<ProductRes>();
            if (products.Count() != 0)
            {
                for (int i = 0; i < products.Count(); i++)
                {
                    var product = products[i];

                    if (product.Id != null && product.Name != null)
                    {
                        responses.Add(
                            new ProductRes { Id = (long)product.Id, Name = product.Name }
                        );
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return responses;
        }

        public async Task<ProductRes?> Add(ProductReq req)
        {
            var product = await _productsRepo.Save(req.Name);

            if (product != null && product.Id != null)
            {
                return new ProductRes { Id = (long)product.Id, Name = product.Name };
            }

            return null;
        }

        public async Task<ProductRes?> Edit(long id, ProductReq req)
        {
            var product = await _productsRepo.Save(req.Name, id);

            if (product != null && product.Id != null)
            {
                return new ProductRes { Id = (long)product.Id, Name = product.Name };
            }

            return null;
        }

        public async Task<ProductRes?> Fetch(long id)
        {
            var product = await _productsRepo.GetById(id);

            if (product != null)
            {
                return new ProductRes { Id = (long)product.Id, Name = product.Name };
            }
            else
            {
                return null;
            }
        }

        public Task<bool> Remove(long id)
        {
            var success = _productsRepo.Remove(id);
            return success;
        }
    }
}
