using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Finance;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
	public interface IIncomeRepository
	{
		Task<List<Income>> GetAll();
		Task AddIncome(Income income);
		Task Save();
		Task Update(Income income);
		Task Delete(int id);
		Task<decimal> GetCurrentMonthTotalIncome();
		Task<IncomeResponse> GetIncomeById(int id);
		Task<decimal> GetTotalIncomeForMonth(DateTime month);
        Task<int> GetIncomeCountByCategory(IncomeType category);
    }
}
