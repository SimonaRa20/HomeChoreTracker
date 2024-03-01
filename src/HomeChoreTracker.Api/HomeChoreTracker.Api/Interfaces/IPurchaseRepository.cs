using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IPurchaseRepository
    {
        Task<List<Purchase>> GetAllPurchases(int homeId);
		Task<Purchase> GetByIdPurchase(int purchaseId);
		Task AddPurchase(Purchase purchase);
        Task<Purchase> GetPurchaseById(int id);
        Task Save();
    }
}
