using HomeChoreTracker.Api.Contracts.HomeChore;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IHomeChoreRepository
    {
        Task AddHomeChoreBase(HomeChoreBase homeChoreBase, int homeId);
        Task CreateHomeChore(HomeChoreRequest homeChoreRequest);
        Task Save();
        Task<List<HomeChoreTask>> GetAll(int id);
        Task Delete(int id);
        Task<HomeChoreTask> Get(int id);
        Task Update(HomeChoreTask homeChoreTask);
    }
}
