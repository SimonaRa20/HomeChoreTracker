using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IUserRepository
    {
        Task<int> GetUserIdByEmail(string inviteeEmail);
        Task<User> GetUserById(int id);
        Task UpdateUser(User user);
        Task<List<User>> GetHomeMembers(int homeId);
        Task<List<BusyInterval>> GetUserBusyIntervals(int userId);
        Task AddBusyInterval(BusyInterval interval);
        Task DeleteInterval(int id);
        Task UpdateInterval(BusyInterval interval);
        Task<BusyInterval> GetBusyIntervalById(int id);
    }
}
