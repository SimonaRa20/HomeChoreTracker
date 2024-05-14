using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IPurchaseRepository
    {
        Task<List<Purchase>> GetAllPurchases(int homeId);
		Task AddPurchase(Purchase purchase);
        Task<Purchase> GetPurchaseById(int id);
        Task<ShoppingItem> GetShoppingItemById(int itemId);
        Task UpdateShoppingItem(ShoppingItem shoppingItem);
        Task<ShoppingItem> GetShoppingItemByTaskId(int taskId);
        Task<List<ShoppingItem>> GetShoppingItemsByTaskId(int taskId);
        Task UpdatePurchase(Purchase purchase);
        Task DeletePurchase(Purchase purchase);
		Task Save();
        Task<bool> CheckOrWasSetAmount(Purchase purchase);
        Task<FinancialRecord> GetRecord(int purchase);
	}
}
