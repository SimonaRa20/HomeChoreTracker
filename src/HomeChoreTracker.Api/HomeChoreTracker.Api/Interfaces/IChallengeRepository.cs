using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IChallengeRepository
    {
        Task AddChallenge(Challenge challenge);
        Task<List<User>> GetUsersOpponents(int userId);
        Task<List<Home>> GetOpponentsHomes(int userId);
        Task<List<Home>> GetUserHomes(int userId);
        Task<Home> GetHome(int homeId);
        Task<User> GetUser(int userId);
        Task<List<Challenge>> GetReceivedChallenges();
        Task<Challenge> GetChallengeById(int challengeId);
        Task Update(Challenge challenge);
	}
}
