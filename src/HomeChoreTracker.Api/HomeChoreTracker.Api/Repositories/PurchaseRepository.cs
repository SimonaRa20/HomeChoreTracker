using DocumentFormat.OpenXml.Spreadsheet;
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
            var purchase = await _dbContext.Purchases
                .Where(x => x.Id.Equals(id))
                .Include(p => p.Items)
                .FirstOrDefaultAsync();

            return purchase;
        }

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ShoppingItem> GetShoppingItemById(int id)
        {
            return await _dbContext.ShoppingItems.FindAsync(id);
        }

        public async Task<ShoppingItem> GetShoppingItemByTaskId(int taskId)
        {
            return await _dbContext.ShoppingItems.Where(x => x.HomeChoreTaskId.Equals(taskId) && x.Time > 0 && x.IsCompleted).FirstOrDefaultAsync();
        }

        public async Task<List<ShoppingItem>> GetShoppingItemsByTaskId(int taskId)
        {
            return await _dbContext.ShoppingItems.Where(x => x.HomeChoreTaskId.Equals(taskId) && x.Time > 0 && x.IsCompleted).ToListAsync();
        }

        public async Task UpdateShoppingItem(ShoppingItem shoppingItem)
        {
            _dbContext.Entry(shoppingItem).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePurchase(Purchase purchase)
        {
            _dbContext.Entry(purchase).State = EntityState.Modified;
        }

        public async Task DeletePurchase(Purchase purchase)
        {
            _dbContext.Purchases.Remove(purchase);
        }

        public async Task<bool> CheckOrWasSetAmount(Purchase purchase)
        {
            bool hasAmount = false;
            var expense = await _dbContext.FinancialRecords.Where(x => x.PurchaseId.Equals(purchase.Id)).FirstOrDefaultAsync();

            if(expense != null)
            {
                hasAmount = true;
            }

            return hasAmount;
        }

		public async Task<FinancialRecord> GetRecord(Purchase purchase)
		{
			return await _dbContext.FinancialRecords.Where(x => x.PurchaseId.Equals(purchase.Id)).FirstOrDefaultAsync(); ;
		}
	}
}
