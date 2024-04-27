using DocumentFormat.OpenXml.Spreadsheet;
using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;
using System.ComponentModel;

namespace HomeChoreTracker.Api.Services
{
	public class NotificationService : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;

		public NotificationService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var currentTime = DateTime.Now;

					if (currentTime.Hour == 7 && currentTime.Minute == 0)
					{
						await SendTodayTasksNotifications();
					}

					if (currentTime.Hour == 20 && currentTime.Minute == 0)
					{
						await SendNextDayTasksNotifications();
					}

					await UpdateCurrentEvents();

					await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
				}
				catch (Exception ex)
				{

				}
			}
		}

		private async Task SendTodayTasksNotifications()
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				var _userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
				var _notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
				var _homeChoreRepository = scope.ServiceProvider.GetRequiredService<IHomeChoreRepository>();

				var users = await _userRepository.GetAllUsers();

				foreach (var user in users)
				{
					var todayTasks = await _homeChoreRepository.GetThisDayAssignedTasks(user.Id, DateTime.Now);

					if (todayTasks.Any())
					{
						string notificationMessage = "Your tasks for today:";
						foreach (var task in todayTasks)
						{
							HomeChoreTask homeChore = await _homeChoreRepository.Get(task.TaskId);

							notificationMessage += $"\n- {homeChore.Name}, {task.StartDate}";
						}

						Notification notification = new Notification
						{
							Title = notificationMessage,
							IsRead = false,
							Time = DateTime.Now,
							UserId = user.Id,
							User = user
						};

						await _notificationRepository.CreateNotification(notification);
					}
				}
			}
		}
		private async Task SendNextDayTasksNotifications()
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				var _userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
				var _notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
				var _homeChoreRepository = scope.ServiceProvider.GetRequiredService<IHomeChoreRepository>();

				var users = await _userRepository.GetAllUsers();

				foreach (var user in users)
				{
					var nextDayTasks = await _homeChoreRepository.GetNextDayAssignedTasks(user.Id, DateTime.Now);

					if (nextDayTasks.Any())
					{
						string notificationMessage = "Your tasks for tomorrow:";
						foreach (var task in nextDayTasks)
						{
							HomeChoreTask homeChore = await _homeChoreRepository.Get(task.TaskId);

							notificationMessage += $"\n- {homeChore.Name}, {task.StartDate}";
						}

						Notification notification = new Notification
						{
							Title = notificationMessage,
							IsRead = false,
							Time = DateTime.Now,
							UserId = user.Id,
							User = user
						};

						await _notificationRepository.CreateNotification(notification);
					}
				}
			}
		}

		private async Task UpdateCurrentEvents()
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				var _challengeRepository = scope.ServiceProvider.GetRequiredService<IChallengeRepository>();
				var _notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

				var challenges = await _challengeRepository.GetCurrentChallenges();

				foreach (var challenge in challenges)
				{
					if(challenge.ResultType == ChallengeResultType.None)
					{
						if(challenge.EndTime < DateTime.Now && challenge.ChallengeCount > challenge.Count &&
						   challenge.ChallengeCount > challenge.OpponentCount)
						{
							challenge.ResultType = ChallengeResultType.Undefeated;
						}
						else if(challenge.ChallengeCount <= challenge.Count)
						{
							challenge.ResultType = ChallengeResultType.Win;
						}
						else if(challenge.ChallengeCount <= challenge.OpponentCount)
						{
							challenge.ResultType = ChallengeResultType.Lost;
						}	
					}

					await _challengeRepository.Update(challenge);

					bool isUndefeated = false;
					if(challenge.ResultType.Equals(ChallengeResultType.Undefeated))
					{
						isUndefeated = true;
					}

					if (challenge.OpponentType.Equals(OpponentType.User) && challenge.ResultType != ChallengeResultType.None)
					{
						User user = await _challengeRepository.GetUser((int)challenge.UserId);
						User opponent = await _challengeRepository.GetUser((int)challenge.OpponentUserId);

						if(isUndefeated)
						{
							Notification notification = new Notification
							{
								Title = "You undefeated the challenge.",
								IsRead = false,
								Time = DateTime.Now,
								UserId = user.Id,
								User = user
							};

							await _notificationRepository.CreateNotification(notification);

							notification.UserId = opponent.Id;
							notification.User = opponent;

							await _notificationRepository.CreateNotification(notification);
						}
						else
						{
							Notification notification = new Notification
							{
								Title = challenge.ResultType == ChallengeResultType.Win ? "Congratulations! You won the challenge." : "You lost the challenge.",
								IsRead = false,
								Time = DateTime.Now,
								UserId = user.Id,
								User = user
							};

							await _notificationRepository.CreateNotification(notification);

							Notification opponentNotification = new Notification
							{
								Title = challenge.ResultType == ChallengeResultType.Win ? "You lost the challenge." : "Congratulations! You won the challenge.",
								IsRead = false,
								Time = DateTime.Now,
								UserId = opponent.Id,
								User = opponent
							};

							await _notificationRepository.CreateNotification(opponentNotification);
						}
						
					}
					else if(challenge.ResultType != ChallengeResultType.None)
					{
						List<User> users = await _challengeRepository.GetUsersByHome((int)challenge.HomeId);
						List<User> opponents = await _challengeRepository.GetUsersByHome((int)challenge.OpponentHomeId);


						foreach(var u in users)
						{
							if (!isUndefeated)
							{
								Notification notification = new Notification
								{
									Title = challenge.ResultType == ChallengeResultType.Win ? "Congratulations! You won the challenge." : "You lost the challenge.",
									IsRead = false,
									Time = DateTime.Now,
									UserId = u.Id,
									User = u
								};

								await _notificationRepository.CreateNotification(notification);
							}
							else
							{
								Notification notification = new Notification
								{
									Title = "You undefeated the challenge.",
									IsRead = false,
									Time = DateTime.Now,
									UserId = u.Id,
									User = u
								};

								await _notificationRepository.CreateNotification(notification);
							}
						}
						
						foreach(var op in opponents)
						{
							if (!isUndefeated)
							{
								Notification opponentNotification = new Notification
								{
									Title = challenge.ResultType == ChallengeResultType.Win ? "You lost the challenge." : "Congratulations! You won the challenge.",
									IsRead = false,
									Time = DateTime.Now,
									UserId = op.Id,
									User = op
								};

								await _notificationRepository.CreateNotification(opponentNotification);
							}
							else
							{
								Notification notification = new Notification
								{
									Title = "You undefeated the challenge.",
									IsRead = false,
									Time = DateTime.Now,
									UserId = op.Id,
									User = op
								};

								await _notificationRepository.CreateNotification(notification);
							}
						}
					}
				}
			}
		}
	}
}
