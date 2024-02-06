using HomeChoreTracker.Api.Contracts.Home;
using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface IHomeRepository
    {
        Task CreateHome(HomeRequest homeRequest, int userId);
        Task<List<Home>> GetAll(int userId);
        Task<string> InviteUserToHome(int inviterUserId, int homeId, string inviteeEmail);
        Task<bool> ValidateInvitationToken(string invitationToken);
        Task AssociateUserWithHome(int userId, string invitationToken, bool invitationAccept);
    }
}
