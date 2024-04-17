using HomeChoreTracker.Api.Contracts.Home;
using HomeChoreTracker.Api.Contracts.User;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IHomeRepository
    {
        Task<Home> CreateHome(HomeRequest homeRequest, int userId);
        Task<bool> CheckOrExistTitle(HomeRequest homeRequest);
        Task Update(Home home);
        Task<List<Home>> GetAll(int userId);
        Task<HomeInvitation> Get(int homeId);
        Task<Home> GetHome(int homeId);
        Task<string> InviteUserToHome(int inviterUserId, int homeId, string inviteeEmail);
        Task<HomeInvitation> GetInvitationByToken(string token);
        Task AddToHome(UserHomes userHomes);
        Task RemoveInvitation(HomeInvitation homeInvitation);
        Task<bool> OrHomeMember(int homeId, int userId);
        Task<List<UserGetResponse>> GetHomeMembers(int homeId);
    }
}
