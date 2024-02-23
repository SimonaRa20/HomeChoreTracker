using HomeChoreTracker.Api.Contracts.Forum;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
	public interface IForumRepository
	{
		Task AddAdvice(Advice advice);
		Task Delete(int id);
		Task<bool> HasPermission(int id, int userId);
		Task<List<AdviceResponse>> GetAll(int userId);
	}
}
