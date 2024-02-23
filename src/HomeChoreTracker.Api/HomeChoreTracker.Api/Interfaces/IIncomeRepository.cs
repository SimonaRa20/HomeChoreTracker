using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
	public interface IIncomeRepository
	{
		Task<List<Income>> GetAll(int userId);
		Task AddIncome(Income income);
		Task Save();
		Task Update(Income income);
		Task Delete(int id);
		Task<decimal> GetCurrentMonthTotalIncome(int userId);
		Task<IncomeResponse> GetIncomeById(int id);
		Task<decimal> GetTotalIncomeForMonth(DateTime month, int userId);
        Task<int> GetIncomeCountByCategory(IncomeType category, int userId);
        Task<List<Income>> GetIncomesByDateRange(DateTime startDate, DateTime endDate, int userId);
    }
}
