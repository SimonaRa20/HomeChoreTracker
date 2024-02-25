using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly HomeChoreTrackerDbContext _dbContext;

        public ProductRepository(HomeChoreTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Product>> GetAllProducts(int id)
        {
            return await _dbContext.Products.Where(x=>x.HomeId.Equals(id)).ToListAsync();
        }

        public async Task AddProduct(Product product)
        {
            await _dbContext.Products.AddAsync(product);
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _dbContext.Products.FindAsync(id);
        }

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
