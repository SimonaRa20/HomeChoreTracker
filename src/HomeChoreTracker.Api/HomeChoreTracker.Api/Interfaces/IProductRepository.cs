using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProducts(int id);
        Task<Product> GetProductByTitleAndType(string title, ProductType productType, int homeId);
        Task UpdateProduct(Product product);
        Task AddProduct(Product product);
        Task<Product> GetProductById(int id);
        Task Save();
    }
}
