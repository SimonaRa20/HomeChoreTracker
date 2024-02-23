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
			Income income = await _dbContext.Incomes.FindAsync(id);

			IncomeResponse incomeResponse = new IncomeResponse
			{
				Title = income.Title,
				Amount = income.Amount,
				Description	= income?.Description ?? "-",
				Time = income.Time,
				Type = income.Type,
				HomeId =income.HomeId
            };

			return incomeResponse;
		}

		public async Task<List<Income>> GetAll(int userId)
		{
			return await _dbContext.Incomes.Where(x=>x.UserId.Equals(userId)).ToListAsync();
		}

		public async Task AddIncome(Income income)
		{
			await _dbContext.Incomes.AddAsync(income);
			await Save();
		}

		public async Task<decimal> GetCurrentMonthTotalIncome(int userId)
		{
			DateTime currentDate = DateTime.Now;
			DateTime startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
			DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

			return await _dbContext.Incomes
				.Where(i => i.UserId.Equals(userId) && i.Time >= startOfMonth && i.Time <= endOfMonth)
				.SumAsync(i => i.Amount);
		}

		public async Task Save()
		{
			await _dbContext.SaveChangesAsync();
		}

		public async Task Update(Income income)
		{
			_dbContext.Entry(income).State = EntityState.Modified;
			await Save();
		}

		public async Task Delete(int id)
		{
			Income income = await _dbContext.Incomes.FindAsync(id);
			if (income != null)
			{
				_dbContext.Incomes.Remove(income);
			}
			await Save();
		}

		public async Task<decimal> GetTotalIncomeForMonth(DateTime month, int userId)
		{
			// Get the start and end dates of the month
			DateTime startDate = new DateTime(month.Year, month.Month, 1);
			DateTime endDate = startDate.AddMonths(1).AddDays(-1);

			// Query the database for total income within the specified month
			decimal totalIncome = await _dbContext.Incomes
				.Where(i => i.UserId.Equals(userId) && i.Time >= startDate && i.Time <= endDate)
				.SumAsync(i => i.Amount);

			return totalIncome;
		}

        public async Task<int> GetIncomeCountByCategory(IncomeType category, int userId)
        {
            return await _dbContext.Incomes.Where(x=>x.UserId.Equals(userId))
                .CountAsync(e => e.Type == category);
        }

        public async Task<List<Income>> GetIncomesByDateRange(DateTime startDate, DateTime endDate, int userId)
        {
            return await _dbContext.Incomes
                .Where(i => i.UserId.Equals(userId) && i.Time >= startDate && i.Time <= endDate)
                .ToListAsync();
        }
    }
}
