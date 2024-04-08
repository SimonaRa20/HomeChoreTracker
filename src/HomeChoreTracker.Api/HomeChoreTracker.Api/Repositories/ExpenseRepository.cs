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
			FinancialRecord expense = await _dbContext.FinancialRecords.FindAsync(id);
			FinancialCategory category = await _dbContext.FinancialCategories.FindAsync(expense.FinancialCategoryId);

			ExpenseResponse expenseResponse = new ExpenseResponse
			{
				Title = expense.Title,
				Amount = expense.Amount,
				Description = expense?.Description ?? "-",
				Time = expense.Time,
				Type = expense.Type,
                FinancialCategory = category.Name,
                FinancialCategoryId = category.Id,
				HomeId = expense.HomeId,
            };

			return expenseResponse;

		}

		public async Task<List<FinancialRecord>> GetAll(int userId)
		{
			return await _dbContext.FinancialRecords.Where(x=> x.Type.Equals(FinancialType.Expense) && x.UserId.Equals(userId)).ToListAsync();
		}

		public async Task AddExpense(FinancialRecord expense)
		{
			await _dbContext.FinancialRecords.AddAsync(expense);
			await Save();
		}

		public async Task Save()
		{
			await _dbContext.SaveChangesAsync();
		}

		public async Task Update(FinancialRecord expense)
		{
			_dbContext.Entry(expense).State = EntityState.Modified;
			await Save();
		}

		public async Task Delete(int id)
		{
			FinancialRecord expense = await _dbContext.FinancialRecords.FindAsync(id);
			if (expense != null)
			{
				_dbContext.FinancialRecords.Remove(expense);
			}
			await Save();
		}

		public async Task<decimal> GetCurrentMonthTotalExpense(int userId)
		{
			DateTime currentDate = DateTime.Now;
			DateTime startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
			DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

			return await _dbContext.FinancialRecords
				.Where(e => e.Type.Equals(FinancialType.Expense) && e.UserId.Equals(userId) && e.Time >= startOfMonth && e.Time <= endOfMonth)
				.SumAsync(e => e.Amount);
		}

		public async Task<decimal> GetTotalExpenseForMonth(DateTime month, int userId)
		{
			DateTime startDate = new DateTime(month.Year, month.Month, 1);
			DateTime endDate = startDate.AddMonths(1).AddDays(-1);

			decimal totalExpense = await _dbContext.FinancialRecords
				.Where(e => e.Type.Equals(FinancialType.Expense) && e.UserId.Equals(userId) && e.Time >= startDate && e.Time <= endDate)
				.SumAsync(e => e.Amount);

			return totalExpense;
		}

        public async Task<int> GetExpenseCountByCategory(int category, int userId)
        {
            return await _dbContext.FinancialRecords.Where(x=> x.Type.Equals(FinancialType.Expense) && x.UserId.Equals(userId))
                .CountAsync(e => e.FinancialCategoryId == category);
        }

        public async Task<List<FinancialRecord>> GetExpensesByDateRange(DateTime startDate, DateTime endDate, int userId)
        {
            return await _dbContext.FinancialRecords
                .Where(e => e.Type.Equals(FinancialType.Expense) && e.UserId.Equals(userId) && e.Time >= startDate && e.Time <= endDate)
                .ToListAsync();
        }

        public async Task<decimal> GetCurrentMonthTotalHomeExpense(int id)
        {
            DateTime currentDate = DateTime.Now;
            DateTime startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            return await _dbContext.FinancialRecords
                .Where(e => e.Type.Equals(FinancialType.Expense) && e.HomeId.Equals(id) && e.Time >= startOfMonth && e.Time <= endOfMonth)
                .SumAsync(e => e.Amount);
        }

        public async Task<List<FinancialRecord>> GetHomeAll(int id)
        {
            return await _dbContext.FinancialRecords.Where(x => x.Type.Equals(FinancialType.Expense) && x.HomeId.Equals(id)).ToListAsync();
        }

        public async Task<decimal> GetTotalHomeExpenseForMonth(DateTime month, int id)
        {
            DateTime startDate = new DateTime(month.Year, month.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            decimal totalExpense = await _dbContext.FinancialRecords
                .Where(e => e.Type.Equals(FinancialType.Expense) && e.HomeId.Equals(id) && e.Time >= startDate && e.Time <= endDate)
                .SumAsync(e => e.Amount);

            return totalExpense;
        }

        public async Task<int> GetHomeExpenseCountByCategory(int category, int id)
        {
            return await _dbContext.FinancialRecords.Where(x => x.Type.Equals(FinancialType.Expense) && x.HomeId.Equals(id))
                .CountAsync(e => e.FinancialCategoryId == category);
        }

		public async Task<List<FinancialCategory>> GetExpenseCategories()
		{
            return await _dbContext.FinancialCategories.Where(x=>x.Type.Equals(FinancialType.Expense)).ToListAsync();
        }

		public async Task<FinancialCategory> GetExpenseCategory(int id)
		{
			return await _dbContext.FinancialCategories.Where(x => x.Type.Equals(FinancialType.Expense) && x.Id.Equals(id)).FirstOrDefaultAsync();
		}

		public async Task<FinancialCategory> CheckCategory(string financialCategory)
		{
			return await _dbContext.FinancialCategories.FirstOrDefaultAsync(x => x.Type.Equals(FinancialType.Expense) && x.Name.Equals(financialCategory));
		}

		public async Task<decimal> GetTotalExpenseForCategory(DateTime startDate, DateTime endDate, int userId, int categoryId)
		{
			return await _dbContext.FinancialRecords
				.Where(e => e.Type.Equals(FinancialType.Expense) && e.UserId.Equals(userId) && e.FinancialCategoryId == categoryId && e.Time >= startDate && e.Time <= endDate)
				.SumAsync(e => e.Amount);
		}
	}
}
