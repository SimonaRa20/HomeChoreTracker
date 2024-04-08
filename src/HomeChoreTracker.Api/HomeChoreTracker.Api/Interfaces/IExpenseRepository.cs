using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
	public interface IExpenseRepository
	{
		Task<List<FinancialRecord>> GetAll(int userId);
        Task<List<FinancialRecord>> GetHomeAll(int id);
        Task AddExpense(FinancialRecord expense);
		Task Save();
		Task Update(FinancialRecord expense);
		Task Delete(int id);
		Task<decimal> GetCurrentMonthTotalExpense(int userId);
        Task<decimal> GetCurrentMonthTotalHomeExpense(int id);
        Task<ExpenseResponse> GetExpenseById(int id);
		Task<decimal> GetTotalExpenseForMonth(DateTime month, int userId);
        Task<decimal> GetTotalHomeExpenseForMonth(DateTime month, int id);
        Task<int> GetExpenseCountByCategory(int category, int userId);
        Task<int> GetHomeExpenseCountByCategory(int category, int id);
        Task<List<FinancialRecord>> GetExpensesByDateRange(DateTime startDate, DateTime endDate, int userId);
        Task<List<FinancialCategory>> GetExpenseCategories();
		Task<FinancialCategory> CheckCategory(string financialCategory);
		Task<FinancialCategory> GetExpenseCategory(int id);
		Task<decimal> GetTotalExpenseForCategory(DateTime startDate, DateTime endDate, int userId, int categoryId);
	}
}
