using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;

namespace HomeChoreTracker.Api.Services
{
    public class GamificationService : BackgroundService
    {
        private readonly IGamificationRepository _gamificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;

        public GamificationService(IGamificationRepository gamificationRepository, IUserRepository userRepository, INotificationRepository notificationRepository)
        {
            _gamificationRepository = gamificationRepository;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var users = await _userRepository.GetAllUsers();

                    foreach (var user in users)
                    {
                        bool hasEarnedFiftyPoints = await _gamificationRepository.UserHasEarnedFiftyPointsPerDay(user.Id);

                        if (!hasEarnedFiftyPoints)
                        {
                            BadgeWallet wallet = await _gamificationRepository.GetUserBadgeWallet(user.Id);
                            wallet.EarnedPerDayFiftyPoints = true;
                            await _gamificationRepository.UpdateBadgeWallet(wallet);

                            Notification noti = new Notification
                            {
                                Title = $"You earned badge 'Earned per day fifty points'",
                                IsRead = false,
                                Time = DateTime.Now,
                                UserId = (int)user.Id,
                                User = user,
                            };

                            await _notificationRepository.CreateNotification(noti);
                        }

                        bool hasEarnedHundredPoints = await _gamificationRepository.UserHasEarnedHundredPointsPerDay(user.Id);

                        if (!hasEarnedFiftyPoints)
                        {
                            BadgeWallet wallet = await _gamificationRepository.GetUserBadgeWallet(user.Id);
                            wallet.EarnedPerDayHundredPoints = true;
                            await _gamificationRepository.UpdateBadgeWallet(wallet);

                            Notification noti = new Notification
                            {
                                Title = $"You earned badge 'Earned per day hundred points'",
                                IsRead = false,
                                Time = DateTime.Now,
                                UserId = (int)user.Id,
                                User = user,
                            };

                            await _notificationRepository.CreateNotification(noti);
                        }
                    }

                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    // Log any exceptions
                    Console.WriteLine($"An error occurred in GamificationService: {ex.Message}");
                }
            }
        }
}
