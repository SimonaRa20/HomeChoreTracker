using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IChallengeRepository
    {
        Task AddChallenge(Challenge challenge);
        Task<List<User>> GetUsersOpponents(int userId);
        Task<List<User>> GetUsersByHome(int homeId);
		Task<List<Home>> GetOpponentsHomes(int userId);
        Task<List<Home>> GetUserHomes(int userId);
        Task<Home> GetHome(int homeId);
        Task<User> GetUser(int userId);
        Task<List<Challenge>> GetReceivedChallenges();
        Task<Challenge> GetChallengeById(int challengeId);
        Task<List<Challenge>> GetHistoryChallenges();
		Task Update(Challenge challenge);
        Task<List<Challenge>> GetCurrentChallenges();
        Task<List<Challenge>> GetChallenges();
        Task UpdateChallenge(TaskAssignment assignment);
        Task<Challenge> UpdateChallengeCount(TaskAssignment assignment, Challenge challenge);
        Task<Challenge> UpdateChallengePoints(Challenge challenge, HomeChoreTask homeChoreTask, bool opponent);
	}
}
