using HomeChoreTracker.Api.Models;

namespace HomeChoreTracker.Api.Interfaces
{
    public interface INotificationRepository
    {
        Task CreateNotification(Notification notification);
        Task<List<Notification>> GetNotifications(int userId);
        Task<List<Notification>> GetNotReadNotifications(int userId);
        Task Update(Notification notification);
    }
}
