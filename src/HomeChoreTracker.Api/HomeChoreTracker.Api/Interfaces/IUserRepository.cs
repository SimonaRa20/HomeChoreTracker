using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IUserRepository
    {
        Task<int> GetUserIdByEmail(string inviteeEmail);
        Task<User> GetUserById(int id);
        Task UpdateUser(User user);
        Task<List<User>> GetHomeMembers(int homeId);
    }
}
