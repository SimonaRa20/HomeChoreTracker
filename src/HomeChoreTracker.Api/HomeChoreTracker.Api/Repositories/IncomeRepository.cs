using DocumentFormat.OpenXml.Spreadsheet;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Repositories
{
	public class IncomeRepository : IIncomeRepository
	{
		private readonly HomeChoreTrackerDbContext _dbContext;

		public IncomeRepository(HomeChoreTrackerDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IncomeResponse> GetIncomeById(int id)
		{
			FinancialRecord income = await _dbContext.FinancialRecords.FindAsync(id);
			FinancialCategory category = await GetIncomeCategory((int)income.FinancialCategoryId);
			IncomeResponse incomeResponse = new IncomeResponse
			{
				Title = income.Title,
				Amount = income.Amount,
				Description	= income?.Description ?? "-",
				Time = income.Time,
				Type = FinancialType.Income,
				FinancialCategory = category.Name,
				FinancialCategoryId = (int)income.FinancialCategoryId,
				HomeId =income.HomeId
            };

			return incomeResponse;
		}

		public async Task<List<FinancialRecord>> GetAll(int userId)
		{
			return await _dbContext.FinancialRecords.Where(x=> x.Type.Equals(FinancialType.Income) && x.UserId.Equals(userId)).ToListAsync();
		}

        public async Task<FinancialCategory> AddCategory(FinancialCategory financialCategory)
        {
            await _dbContext.FinancialCategories.AddAsync(financialCategory);
            await Save();
			return financialCategory;
        }


        public async Task AddIncome(FinancialRecord income)
		{
			await _dbContext.FinancialRecords.AddAsync(income);
			await Save();
		}

		public async Task<decimal> GetCurrentMonthTotalIncome(int userId)
		{
			DateTime currentDate = DateTime.Now;
			DateTime startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
			DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

			return await _dbContext.FinancialRecords
				.Where(i => i.Type.Equals(FinancialType.Income) && i.UserId.Equals(userId) && i.Time >= startOfMonth && i.Time <= endOfMonth)
				.SumAsync(i => i.Amount);
		}

		public async Task Save()
		{
			await _dbContext.SaveChangesAsync();
		}

		public async Task Update(FinancialRecord income)
		{
			_dbContext.Entry(income).State = EntityState.Modified;
			await Save();
		}

		public async Task Delete(int id)
		{
			FinancialRecord income = await _dbContext.FinancialRecords.FindAsync(id);
			if (income != null)
			{
				_dbContext.FinancialRecords.Remove(income);
			}
			await Save();
		}

		public async Task<decimal> GetTotalIncomeForMonth(DateTime month, int userId)
		{
			// Get the start and end dates of the month
			DateTime startDate = new DateTime(month.Year, month.Month, 1);
			DateTime endDate = startDate.AddMonths(1).AddDays(-1);

			// Query the database for total income within the specified month
			decimal totalIncome = await _dbContext.FinancialRecords
				.Where(i => i.Type.Equals(FinancialType.Income) && i.UserId.Equals(userId) && i.Time >= startDate && i.Time <= endDate)
				.SumAsync(i => i.Amount);

			return totalIncome;
		}

        public async Task<int> GetIncomeCountByCategory(int category, int userId)
        {
            return await _dbContext.FinancialRecords.Where(x=> x.Type.Equals(FinancialType.Income) && x.UserId.Equals(userId))
                .CountAsync(e => e.FinancialCategoryId == category);
        }

        public async Task<List<FinancialRecord>> GetIncomesByDateRange(DateTime startDate, DateTime endDate, int userId)
        {
            return await _dbContext.FinancialRecords
                .Where(i => i.Type.Equals(FinancialType.Income) && i.UserId.Equals(userId) && i.Time >= startDate && i.Time <= endDate)
                .ToListAsync();
        }

        public async Task<decimal> GetCurrentMonthTotalHomeIncome(int id)
        {
            DateTime currentDate = DateTime.Now;
            DateTime startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            return await _dbContext.FinancialRecords
                .Where(i => i.Type.Equals(FinancialType.Income) && i.HomeId.Equals(id) && i.Time >= startOfMonth && i.Time <= endOfMonth)
                .SumAsync(i => i.Amount);
        }

        public async Task<List<FinancialRecord>> GetHomeAll(int id)
        {
            return await _dbContext.FinancialRecords.Where(x => x.Type.Equals(FinancialType.Income) && x.HomeId.Equals(id)).ToListAsync();
        }

        public async Task<decimal> GetTotalHomeIncomeForMonth(DateTime month, int id)
        {
            // Get the start and end dates of the month
            DateTime startDate = new DateTime(month.Year, month.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            // Query the database for total income within the specified month
            decimal totalIncome = await _dbContext.FinancialRecords
                .Where(i => i.Type.Equals(FinancialType.Income) && i.HomeId.Equals(id) && i.Time >= startDate && i.Time <= endDate)
                .SumAsync(i => i.Amount);

            return totalIncome;
        }

        public async Task<int> GetHomeIncomeCountByCategory(int category, int id)
        {
            return await _dbContext.FinancialRecords.Where(x => x.Type.Equals(FinancialType.Income) && x.HomeId.Equals(id))
                .CountAsync(e => e.FinancialCategoryId == category);
        }

        public async Task<List<FinancialCategory>> GetIncomeCategories()
        {
            return await _dbContext.FinancialCategories.Where(x => x.Type.Equals(FinancialType.Income)).ToListAsync();
        }

		public async Task<FinancialCategory> GetIncomeCategory(int id)
		{
			return await _dbContext.FinancialCategories.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
		}
	}
}
