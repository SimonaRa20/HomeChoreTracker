using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
	public interface IAvatarRepository
	{
		Task AddAvatar(Avatar avatar);
		Task<List<Avatar>> GetAll();
		Task<Avatar>GetAvatar(int id);
		Task Update(Avatar avatar);
	}
}
