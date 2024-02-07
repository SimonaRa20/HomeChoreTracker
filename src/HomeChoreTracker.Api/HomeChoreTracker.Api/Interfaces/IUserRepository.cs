using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IUserRepository
    {
        Task<int> GetUserIdByEmail(string inviteeEmail);
    }
}
