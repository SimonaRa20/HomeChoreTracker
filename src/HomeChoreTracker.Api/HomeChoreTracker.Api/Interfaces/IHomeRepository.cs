using HomeChoreTracker.Api.Contracts.Home;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IHomeRepository
    {
        Task CreateHome(HomeRequest homeRequest, int userId);
        Task<List<Home>> GetAll(int userId);
    }
}
