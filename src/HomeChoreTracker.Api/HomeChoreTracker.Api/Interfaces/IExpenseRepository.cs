using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
	public interface IExpenseRepository
	{
		Task<List<Expense>> GetAll(int userId);
        Task<List<Expense>> GetHomeAll(int id);
        Task AddExpense(Expense expense);
		Task Save();
		Task Update(Expense expense);
		Task Delete(int id);
		Task<decimal> GetCurrentMonthTotalExpense(int userId);
        Task<decimal> GetCurrentMonthTotalHomeExpense(int id);
        Task<ExpenseResponse> GetExpenseById(int id);
		Task<decimal> GetTotalExpenseForMonth(DateTime month, int userId);
        Task<decimal> GetTotalHomeExpenseForMonth(DateTime month, int id);
        Task<int> GetExpenseCountByCategory(ExpenseType category, int userId);
        Task<int> GetHomeExpenseCountByCategory(ExpenseType category, int id);
        Task<List<Expense>> GetExpensesByDateRange(DateTime startDate, DateTime endDate, int userId);
    }
}
