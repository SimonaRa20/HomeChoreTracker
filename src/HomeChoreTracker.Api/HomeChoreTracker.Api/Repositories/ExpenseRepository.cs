using DocumentFormat.OpenXml.Spreadsheet;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Finance;
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

		public async Task<ExpenseResponse> GetExpenseById(int id)
		{
			Expense expense = await _dbContext.Expenses.FindAsync(id);

			ExpenseResponse expenseResponse = new ExpenseResponse
			{
				Title = expense.Title,
				Amount = expense.Amount,
				Description = expense?.Description ?? "-",
				Time = expense.Time,
				Type = expense.Type,
				SubscriptionDuration = expense.SubscriptionDuration ?? 0,
				HomeId = expense.HomeId,
            };

			return expenseResponse;

		}

		public async Task<List<Expense>> GetAll(int userId)
		{
			return await _dbContext.Expenses.Where(x=>x.UserId.Equals(userId)).ToListAsync();
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

		public async Task<decimal> GetCurrentMonthTotalExpense(int userId)
		{
			DateTime currentDate = DateTime.Now;
			DateTime startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
			DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

			return await _dbContext.Expenses
				.Where(e => e.UserId.Equals(userId) && e.Time >= startOfMonth && e.Time <= endOfMonth)
				.SumAsync(e => e.Amount);
		}

		public async Task<decimal> GetTotalExpenseForMonth(DateTime month, int userId)
		{
			DateTime startDate = new DateTime(month.Year, month.Month, 1);
			DateTime endDate = startDate.AddMonths(1).AddDays(-1);

			decimal totalExpense = await _dbContext.Expenses
				.Where(e => e.UserId.Equals(userId) && e.Time >= startDate && e.Time <= endDate)
				.SumAsync(e => e.Amount);

			return totalExpense;
		}

        public async Task<int> GetExpenseCountByCategory(ExpenseType category, int userId)
        {
            return await _dbContext.Expenses.Where(x=>x.UserId.Equals(userId))
                .CountAsync(e => e.Type == category);
        }

        public async Task<List<Expense>> GetExpensesByDateRange(DateTime startDate, DateTime endDate, int userId)
        {
            return await _dbContext.Expenses
                .Where(e => e.UserId.Equals(userId) && e.Time >= startDate && e.Time <= endDate)
                .ToListAsync();
        }

        public async Task<decimal> GetCurrentMonthTotalHomeExpense(int id)
        {
            DateTime currentDate = DateTime.Now;
            DateTime startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            return await _dbContext.Expenses
                .Where(e => e.HomeId.Equals(id) && e.Time >= startOfMonth && e.Time <= endOfMonth)
                .SumAsync(e => e.Amount);
        }

        public async Task<List<Expense>> GetHomeAll(int id)
        {
            return await _dbContext.Expenses.Where(x => x.HomeId.Equals(id)).ToListAsync();
        }

        public async Task<decimal> GetTotalHomeExpenseForMonth(DateTime month, int id)
        {
            DateTime startDate = new DateTime(month.Year, month.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            decimal totalExpense = await _dbContext.Expenses
                .Where(e => e.HomeId.Equals(id) && e.Time >= startDate && e.Time <= endDate)
                .SumAsync(e => e.Amount);

            return totalExpense;
        }

        public async Task<int> GetHomeExpenseCountByCategory(ExpenseType category, int id)
        {
            return await _dbContext.Expenses.Where(x => x.HomeId.Equals(id))
                .CountAsync(e => e.Type == category);
        }
    }
}
