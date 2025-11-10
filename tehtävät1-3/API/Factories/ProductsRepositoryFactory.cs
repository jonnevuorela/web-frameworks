using API.Interfaces;
using API.Repositories;

namespace API.Factories
{
    public static class ProductsRepositoryFactory
    {
        public static IProductsRepository Create()
        {
            return new ProductsSQLiteRepository();
        }
    }
}
