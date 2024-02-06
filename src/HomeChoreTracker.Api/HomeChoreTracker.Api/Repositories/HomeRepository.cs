using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Home;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Database;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace HomeChoreTracker.Api.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly HomeChoreTrackerDbContext _dbContext;
        private readonly IUserRepository _userRepository;

        public HomeRepository(HomeChoreTrackerDbContext dbContext, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        public async Task CreateHome(HomeRequest homeRequest, int userId)
        {
            Home home = new Home
            {
                Title = homeRequest.Title,
            };

            await _dbContext.Homes.AddAsync(home);
            await _dbContext.SaveChangesAsync();

            var userHome = new UserHomes
            {
                UserId = userId,
                HomeId = home.Id,
                HomeRole = HomeRole.HomeAdmin,
            };

            _dbContext.UserHomes.Add(userHome);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Home>> GetAll(int userId)
        {
            return await _dbContext.UserHomes.Where(x => x.UserId == userId).Select(h => h.Home).ToListAsync();
        }

        public async Task<string> InviteUserToHome(int inviterUserId, int homeId, string inviteeEmail)
        {
            var isAdmin = await _dbContext.UserHomes
                .AnyAsync(uh => uh.UserId == inviterUserId && uh.HomeId == homeId && uh.HomeRole == HomeRole.HomeAdmin);

            if (!isAdmin)
            {
                throw new InvalidOperationException("Only HomeAdmin can invite users to the home.");
            }

            var token = GenerateInvitationToken();

            var invitation = new HomeInvitation
            {
                HomeId = homeId,
                InviterUserId = inviterUserId,
                InviteeEmail = inviteeEmail,
                InvitationToken = token,
                ExpirationDate = DateTime.UtcNow.AddDays(7)
            };

            _dbContext.HomeInvitations.Add(invitation);
            await _dbContext.SaveChangesAsync();

            var invitedUserId = await _userRepository.GetUserIdByEmail(inviteeEmail);

            if (invitedUserId != null)
            {
                await SendInvitationNotification(invitedUserId, $"You have been invited to join a home group.");
            }


            return token;
        }

        private async Task SendInvitationNotification(int userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Created = DateTime.UtcNow,
            };

            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();
        }

        private string GenerateInvitationToken()
        {
            const int tokenLength = 8;

            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[tokenLength];
                rng.GetBytes(tokenData);

                // Convert to a string using Base64 encoding
                string token = Convert.ToBase64String(tokenData);

                // Remove non-alphanumeric characters to ensure it's 8 characters
                token = new string(token.Where(char.IsLetterOrDigit).ToArray());

                // Ensure the token is exactly 8 characters
                return token.Length >= tokenLength ? token.Substring(0, tokenLength) : token.PadRight(tokenLength, '0');
            }
        }

        public async Task<bool> ValidateInvitationToken(string invitationToken)
        {
            var invitation = await _dbContext.HomeInvitations
                .FirstOrDefaultAsync(i => i.InvitationToken == invitationToken);

            return invitation != null && !IsInvitationExpired(invitation);
        }

        private bool IsInvitationExpired(HomeInvitation invitation)
        {
            return invitation.ExpirationDate < DateTime.UtcNow;
        }

        public async Task AssociateUserWithHome(int userId, string invitationToken, bool acceptInvitation)
        {
            var invitation = await _dbContext.HomeInvitations
                .Include(i => i.Home)
                .FirstOrDefaultAsync(i => i.InvitationToken == invitationToken);

            if (invitation == null || IsInvitationExpired(invitation) || invitation.IsAccepted)
            {
                throw new InvalidOperationException("Invalid or expired invitation token.");
            }

            if (acceptInvitation)
            {
                var newUserHome = new UserHomes
                {
                    UserId = userId,
                    HomeId = invitation.HomeId,
                    HomeRole = HomeRole.HomeUser,
                };

                _dbContext.UserHomes.Add(newUserHome);
                invitation.IsAccepted = true;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                _dbContext.HomeInvitations.Remove(invitation);
                await _dbContext.SaveChangesAsync();
            }
        }
        

    }
}
