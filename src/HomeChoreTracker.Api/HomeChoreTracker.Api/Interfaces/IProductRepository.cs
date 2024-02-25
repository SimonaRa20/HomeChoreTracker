using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProducts(int id);
        Task AddProduct(Product product);
        Task<Product> GetProductById(int id);
        Task Save();
    }
}
