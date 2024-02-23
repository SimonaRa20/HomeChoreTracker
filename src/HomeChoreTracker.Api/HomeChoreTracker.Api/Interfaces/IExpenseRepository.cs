using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
	public interface IExpenseRepository
	{
		Task<List<Expense>> GetAll(int userId);
		Task AddExpense(Expense expense);
		Task Save();
		Task Update(Expense expense);
		Task Delete(int id);
		Task<decimal> GetCurrentMonthTotalExpense(int userId);
		Task<ExpenseResponse> GetExpenseById(int id);
		Task<decimal> GetTotalExpenseForMonth(DateTime month, int userId);
		Task<int> GetExpenseCountByCategory(ExpenseType category, int userId);
        Task<List<Expense>> GetExpensesByDateRange(DateTime startDate, DateTime endDate, int userId);
    }
}
