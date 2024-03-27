using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
	public interface IIncomeRepository
	{
		Task<List<Income>> GetAll(int userId);
        Task<List<Income>> GetHomeAll(int id);
        Task AddIncome(Income income);
		Task Save();
		Task Update(Income income);
		Task Delete(int id);
		Task<decimal> GetCurrentMonthTotalIncome(int userId);
        Task<decimal> GetCurrentMonthTotalHomeIncome(int id);
        Task<IncomeResponse> GetIncomeById(int id);
		Task<decimal> GetTotalIncomeForMonth(DateTime month, int userId);
        Task<decimal> GetTotalHomeIncomeForMonth(DateTime month, int id);
        Task<int> GetIncomeCountByCategory(IncomeType category, int userId);
        Task<int> GetHomeIncomeCountByCategory(IncomeType category, int id);
        Task<List<Income>> GetIncomesByDateRange(DateTime startDate, DateTime endDate, int userId);
    }
}
