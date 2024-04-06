using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using HomeChoreTracker.Api.Repositories;

namespace HomeChoreTracker.Api.Services
{
    public class GamificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public GamificationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _gamificationRepository = scope.ServiceProvider.GetRequiredService<IGamificationRepository>();
                var _userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var _notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                var _homeChoreRepository = scope.ServiceProvider.GetRequiredService<IHomeChoreRepository>();

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var users = await _userRepository.GetAllUsers();

                        foreach (var user in users)
                        {
                            bool hasEarnedFiftyPointsBadge = await _gamificationRepository.UserHasEarnedPerDayFiftyPointsBadge(user.Id);
                            if(!hasEarnedFiftyPointsBadge)
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
                            }

                            bool hasEarnedHundredPointsBadge = await _gamificationRepository.UserHasEarnedPerDayHundredPointsBadge(user.Id);
                            if(!hasEarnedHundredPointsBadge)
                            {
                                bool hasEarnedHundredPoints = await _gamificationRepository.UserHasEarnedHundredPointsPerDay(user.Id);

                                if (!hasEarnedHundredPoints)
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

                            List<TaskAssignment> doneTaskAssignments = await _homeChoreRepository.GetDoneTaskAssigments(user.Id);

                            int totalDoneTasksThisWeek = doneTaskAssignments
                                .Count(task => task.EndDate >= DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek) && task.EndDate <= DateTime.Today.AddDays(7 - (int)DateTime.Today.DayOfWeek));

                            bool hasDoneFiveTaskPerWeek = await _gamificationRepository.UserHasDoneFiveTaskPerWeekBadge(user.Id);

                            if (!hasDoneFiveTaskPerWeek)
                            {
                                if (totalDoneTasksThisWeek >= 5)
                                {
                                    BadgeWallet wallet = await _gamificationRepository.GetUserBadgeWallet(user.Id);
                                    wallet.DoneFiveTaskPerWeek = true;
                                    await _gamificationRepository.UpdateBadgeWallet(wallet);

                                    Notification noti = new Notification
                                    {
                                        Title = $"You earned badge 'Done five tasks per week'",
                                        IsRead = false,
                                        Time = DateTime.Now,
                                        UserId = (int)user.Id,
                                        User = user,
                                    };

                                    await _notificationRepository.CreateNotification(noti);
                                }
                            }

                            bool hasDoneTenTaskPerWeek = await _gamificationRepository.UserHasDoneTenTaskPerWeekBadge(user.Id);

                            if (!hasDoneTenTaskPerWeek)
                            {
                                if (totalDoneTasksThisWeek >= 10)
                                {
                                    BadgeWallet wallet = await _gamificationRepository.GetUserBadgeWallet(user.Id);
                                    wallet.DoneTenTaskPerWeek = true;
                                    await _gamificationRepository.UpdateBadgeWallet(wallet);

                                    Notification noti = new Notification
                                    {
                                        Title = $"You earned badge 'Done ten tasks per week'",
                                        IsRead = false,
                                        Time = DateTime.Now,
                                        UserId = (int)user.Id,
                                        User = user,
                                    };

                                    await _notificationRepository.CreateNotification(noti);
                                }
                            }

                            bool hasDoneTwentyFiveTaskPerWeek = await _gamificationRepository.UserHasDoneTwentyFiveTaskPerWeekBadge(user.Id);

                            if (!hasDoneTwentyFiveTaskPerWeek)
                            {
                                if (totalDoneTasksThisWeek >= 25)
                                {
                                    BadgeWallet wallet = await _gamificationRepository.GetUserBadgeWallet(user.Id);
                                    wallet.DoneTenTaskPerWeek = true;
                                    await _gamificationRepository.UpdateBadgeWallet(wallet);

                                    Notification noti = new Notification
                                    {
                                        Title = $"You earned badge 'Done twenty five tasks per week'",
                                        IsRead = false,
                                        Time = DateTime.Now,
                                        UserId = (int)user.Id,
                                        User = user,
                                    };

                                    await _notificationRepository.CreateNotification(noti);
                                }
                            }
                        }

                        await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred in GamificationService: {ex.Message}");
                    }
                }
            }
        }
    }
}
