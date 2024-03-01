using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly HomeChoreTrackerDbContext _dbContext;

        public PurchaseRepository(HomeChoreTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Purchase>> GetAllPurchases(int homeId)
        {
			return await _dbContext.Purchases.Include(p => p.Items)
                .Where(p => p.HomeId == homeId)
                .ToListAsync();
		}

        public async Task AddPurchase(Purchase purchase)
        {
            await _dbContext.Purchases.AddAsync(purchase);
        }

        public async Task<Purchase> GetPurchaseById(int id)
        {
            return await _dbContext.Purchases.FindAsync(id);
        }

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }

		public async Task<Purchase> GetByIdPurchase(int purchaseId)
		{
			return await _dbContext.Purchases.Where(x=>x.Id.Equals(purchaseId)).Include(p => p.Items).FirstOrDefaultAsync();
		}
	}
}
