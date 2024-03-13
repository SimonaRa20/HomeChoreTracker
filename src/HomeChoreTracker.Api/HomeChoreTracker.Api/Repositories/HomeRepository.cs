using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Contracts.Home;
using HomeChoreTracker.Api.Contracts.HomeChoreBase;
using HomeChoreTracker.Api.Contracts.User;
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

        public async Task<HomeInvitation> Get(int homeId)
        {
            return await _dbContext.HomeInvitations.FirstOrDefaultAsync(x => x.HomeId == homeId);
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

            return token;
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

        public async Task<HomeInvitation> GetInvitationByToken(string token)
        {
            return await _dbContext.HomeInvitations.FirstOrDefaultAsync(x => x.InvitationToken.Equals(token));
        }

        public async Task AddToHome(UserHomes userHomes)
        {
            await _dbContext.UserHomes.AddAsync(userHomes);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveInvitation(HomeInvitation homeInvitation)
        {
            _dbContext.HomeInvitations.Remove(homeInvitation);
            await _dbContext.SaveChangesAsync();
        }

        public async  Task<List<UserGetResponse>> GetHomeMembers(int homeId)
        {
            var homeMembers = await _dbContext.UserHomes
               .Where(uh => uh.HomeId == homeId)
               .Include(uh => uh.User)
               .Select(uh => new UserGetResponse
               {
                   UserName = uh.User.UserName,
                   Email = uh.User.Email,
                   Role = uh.HomeRole
               })
               .ToListAsync();

            return homeMembers;
        }

        public async Task<bool> OrHomeMember (int homeId, int userId)
        {
            return await _dbContext.UserHomes
                .AnyAsync(uh => uh.UserId == userId && uh.HomeId == homeId);
        }

        public async Task<bool> CheckOrExistTitle(HomeRequest homeRequest)
        {
            var home = await _dbContext.Homes.Where(x => x.Title.Equals(homeRequest.Title)).FirstOrDefaultAsync();
            if(home == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
