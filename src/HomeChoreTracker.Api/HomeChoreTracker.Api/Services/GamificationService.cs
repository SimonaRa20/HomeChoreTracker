using HomeChoreTracker.Api.Constants;
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

                                if (hasEarnedFiftyPoints)
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
                            if(hasEarnedHundredPointsBadge)
                            {
                                bool hasEarnedHundredPoints = await _gamificationRepository.UserHasEarnedHundredPointsPerDay(user.Id);

                                if (hasEarnedHundredPoints)
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

                        DateTime time = DateTime.Now.AddDays(-1);

                        List<TaskAssignment> tasks = await _homeChoreRepository.GetDoneTasks(time);

                        foreach(TaskAssignment task in tasks)
                        {
							var homeChore = await _homeChoreRepository.Get(task.TaskId);
							var user = await _userRepository.GetUserById((int)task.HomeMemberId);
							task.IsDone = true;
							var pointHistory = await _gamificationRepository.GetPointsHistoryByTaskId(task.Id);
							BadgeWallet wallet = await _gamificationRepository.GetUserBadgeWallet(user.Id);
							if (pointHistory == null)
							{
								task.Points = homeChore.Points;
								Notification notification = new Notification
								{
									Title = $"Done '{homeChore.Name}' and earned {task.Points} points",
									IsRead = false,
									Time = DateTime.Now,
									UserId = (int)task.HomeMemberId,
									User = user,
								};

								await _notificationRepository.CreateNotification(notification);

								PointsHistory pointsHistory = new PointsHistory
								{
									EarnedPoints = homeChore.Points,
									HomeMemberId = (int)task.HomeMemberId,
									TaskId = (int)task.Id,
									HomeId = homeChore.HomeId,
									Text = $"Done '{homeChore.Name}'",
									EarnedDate = DateTime.Now,
								};

								await _gamificationRepository.AddPointsHistory(pointsHistory);

								var hasBadge = await _gamificationRepository.UserHasDoneFirstTaskBadge(user.Id);
								if (!hasBadge)
								{

									wallet.DoneFirstTask = true;
									await _gamificationRepository.UpdateBadgeWallet(wallet);

									Notification noti = new Notification
									{
										Title = $"You earned badge 'Done first task'",
										IsRead = false,
										Time = DateTime.Now,
										UserId = (int)user.Id,
										User = user,
									};

									await _notificationRepository.CreateNotification(noti);
								}

								if (homeChore.ChoreType.Equals(HomeChoreType.Cleaning))
								{
									var hasDoneFirstCleaningTaskBadge = await _gamificationRepository.UserHasDoneFirstCleaningTaskBadge(user.Id);
									if (!hasDoneFirstCleaningTaskBadge)
									{
										wallet.DoneFirstCleaningTask = true;
										await _gamificationRepository.UpdateBadgeWallet(wallet);

										Notification noti = new Notification
										{
											Title = $"You earned badge 'Done first cleaning task'",
											IsRead = false,
											Time = DateTime.Now,
											UserId = (int)user.Id,
											User = user,
										};

										await _notificationRepository.CreateNotification(noti);
									}
								}
								else if (homeChore.ChoreType.Equals(HomeChoreType.Laundry))
								{
									var hasDoneFirstLaundryTaskBadge = await _gamificationRepository.UserHasDoneFirstLaundryTaskBadge(user.Id);
									if (!hasDoneFirstLaundryTaskBadge)
									{
										wallet.DoneFirstLaundryTask = true;
										await _gamificationRepository.UpdateBadgeWallet(wallet);

										Notification noti = new Notification
										{
											Title = $"You earned badge 'Done first laundry task'",
											IsRead = false,
											Time = DateTime.Now,
											UserId = (int)user.Id,
											User = user,
										};

										await _notificationRepository.CreateNotification(noti);
									}
								}
								else if (homeChore.ChoreType.Equals(HomeChoreType.Kitchen))
								{
									var hasDoneFirstKitchenTaskBadge = await _gamificationRepository.UserHasDoneFirstKitchenTaskBadge(user.Id);
									if (!hasDoneFirstKitchenTaskBadge)
									{
										wallet.DoneFirstKitchenTask = true;
										await _gamificationRepository.UpdateBadgeWallet(wallet);

										Notification noti = new Notification
										{
											Title = $"You earned badge 'Done first kitchen task'",
											IsRead = false,
											Time = DateTime.Now,
											UserId = (int)user.Id,
											User = user,
										};

										await _notificationRepository.CreateNotification(noti);
									}
								}
								else if (homeChore.ChoreType.Equals(HomeChoreType.Bathroom))
								{
									var hasDoneFirstBathroomTaskBadge = await _gamificationRepository.UserHasDoneFirstBathroomTaskBadge(user.Id);
									if (!hasDoneFirstBathroomTaskBadge)
									{
										wallet.DoneFirstBathroomTask = true;
										await _gamificationRepository.UpdateBadgeWallet(wallet);

										Notification noti = new Notification
										{
											Title = $"You earned badge 'Done first bathroom task'",
											IsRead = false,
											Time = DateTime.Now,
											UserId = (int)user.Id,
											User = user,
										};

										await _notificationRepository.CreateNotification(noti);
									}
								}
								else if (homeChore.ChoreType.Equals(HomeChoreType.Bedroom))
								{
									var hasDoneFirstBedroomTaskBadge = await _gamificationRepository.UserHasDoneFirstBedroomTaskBadge(user.Id);
									if (!hasDoneFirstBedroomTaskBadge)
									{
										wallet.DoneFirstBedroomTask = true;
										await _gamificationRepository.UpdateBadgeWallet(wallet);

										Notification noti = new Notification
										{
											Title = $"You earned badge 'Done first bedroom task'",
											IsRead = false,
											Time = DateTime.Now,
											UserId = (int)user.Id,
											User = user,
										};

										await _notificationRepository.CreateNotification(noti);
									}
								}
								else if (homeChore.ChoreType.Equals(HomeChoreType.Outdoors))
								{
									var hasDoneFirstOutdoorsTaskBadge = await _gamificationRepository.UserHasDoneFirstOutdoorsTaskBadge(user.Id);
									if (!hasDoneFirstOutdoorsTaskBadge)
									{
										wallet.DoneFirstOutdoorsTask = true;
										await _gamificationRepository.UpdateBadgeWallet(wallet);

										Notification noti = new Notification
										{
											Title = $"You earned badge 'Done first outdoors task'",
											IsRead = false,
											Time = DateTime.Now,
											UserId = (int)user.Id,
											User = user,
										};

										await _notificationRepository.CreateNotification(noti);
									}
								}
								else
								{
									var hasDoneFirstOrganizeTaskBadge = await _gamificationRepository.UserHasDoneFirstOrganizeTaskBadge(user.Id);
									if (!hasDoneFirstOrganizeTaskBadge)
									{
										wallet.DoneFirstOrganizeTask = true;
										await _gamificationRepository.UpdateBadgeWallet(wallet);

										Notification noti = new Notification
										{
											Title = $"You earned badge 'Done first organize task'",
											IsRead = false,
											Time = DateTime.Now,
											UserId = (int)user.Id,
											User = user,
										};

										await _notificationRepository.CreateNotification(noti);
									}
								}
							}

							await _homeChoreRepository.UpdateTaskAssignment(task);
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
