using HomeChoreTracker.Api.Constants;
using HomeChoreTracker.Api.Interfaces;
using HomeChoreTracker.Api.Models;

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
	}
}
