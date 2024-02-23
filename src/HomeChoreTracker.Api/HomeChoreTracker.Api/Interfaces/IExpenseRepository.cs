using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
	public interface IExpenseRepository
	{
		Task<List<Expense>> GetAll();
		Task AddExpense(Expense expense);
		Task Save();
		Task Update(Expense expense);
		Task Delete(int id);
		Task<decimal> GetCurrentMonthTotalExpense();
		Task<ExpenseResponse> GetExpenseById(int id);
		Task<decimal> GetTotalExpenseForMonth(DateTime month);
		Task<int> GetExpenseCountByCategory(ExpenseType category);
    }
}
