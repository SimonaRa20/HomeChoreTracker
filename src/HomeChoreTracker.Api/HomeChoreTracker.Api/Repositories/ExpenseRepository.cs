using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Repositories
{
	public class ExpenseRepository : IExpenseRepository
	{
		private readonly HomeChoreTrackerDbContext _dbContext;

		public ExpenseRepository(HomeChoreTrackerDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<Expense> GetExpenseById(int id)
		{
			return await _dbContext.Expenses.FindAsync(id);
		}

		public async Task<List<Expense>> GetAll()
		{
			return await _dbContext.Expenses.ToListAsync();
		}

		public async Task AddExpense(Expense expense)
		{
			await _dbContext.Expenses.AddAsync(expense);
			await Save();
		}

		public async Task Save()
		{
			await _dbContext.SaveChangesAsync();
		}

		public async Task Update(Expense expense)
		{
			_dbContext.Entry(expense).State = EntityState.Modified;
			await Save();
		}

		public async Task Delete(int id)
		{
			Expense expense = await _dbContext.Expenses.FindAsync(id);
			if (expense != null)
			{
				_dbContext.Expenses.Remove(expense);
			}
			await Save();
		}

		public async Task<decimal> GetCurrentMonthTotalExpense()
		{
			DateTime currentDate = DateTime.Now;
			DateTime startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
			DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

			return await _dbContext.Expenses
				.Where(e => e.Time >= startOfMonth && e.Time <= endOfMonth)
				.SumAsync(e => e.Amount);
		}
	}
}
