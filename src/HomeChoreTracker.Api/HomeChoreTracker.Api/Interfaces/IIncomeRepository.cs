using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
	public interface IIncomeRepository
	{
		Task<List<FinancialRecord>> GetAll(int userId);
        Task<List<FinancialRecord>> GetHomeAll(int id);
        Task AddIncome(FinancialRecord income);
		Task Save();
		Task Update(FinancialRecord income);
		Task Delete(int id);
		Task<decimal> GetCurrentMonthTotalIncome(int userId);
        Task<decimal> GetCurrentMonthTotalHomeIncome(int id);
		Task<decimal> GetCurrentMonthTotalHomeChores(int id);
		Task<IncomeResponse> GetIncomeById(int id);
		Task<decimal> GetTotalIncomeForMonth(DateTime month, int userId);
        Task<decimal> GetTotalHomeIncomeForMonth(DateTime month, int id);
        Task<int> GetIncomeCountByCategory(int category, int userId);
        Task<int> GetHomeIncomeCountByCategory(int category, int id);
        Task<List<FinancialRecord>> GetIncomesByDateRange(DateTime startDate, DateTime endDate, int userId);
        Task<FinancialCategory> AddCategory(FinancialCategory financialCategory);
		Task<FinancialCategory> CheckCategory(string financialCategory);
		Task<List<FinancialCategory>> GetIncomeCategories();
		Task<FinancialCategory> GetIncomeCategory(int id);
		Task<decimal> GetTotalIncomeForCategory(DateTime startDate, DateTime endDate, int userId, int categoryId);
	}
}
