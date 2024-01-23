using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserById(int id);
        Task AddUser(User user);
        Task Save();
    }
}
