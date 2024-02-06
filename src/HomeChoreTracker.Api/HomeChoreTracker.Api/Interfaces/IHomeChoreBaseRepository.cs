using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IHomeChoreBaseRepository
    {
        Task<List<HomeChoreBase>> GetAll();
        Task AddHomeChoreBase(HomeChoreBaseRequest homeChoreBase);
        Task<HomeChoreBase> GetChoreBase(int id);
        Task Save();
        Task Update(HomeChoreBase homeChoreBase);
        Task Delete(int id);
        Task<List<HomeChoreBase>> GetPaginated(int skip, int take);
    }
}
