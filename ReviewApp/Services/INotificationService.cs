using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModel;
using ReviewApp.Repository;

namespace ReviewApp.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetNotificationsAsync(int userId);
        Task<Notification> GetNotificationByIdAsync(int notificationId);
        Task<int> AddNotificationAsync(Notification notification);
        Task UpdateNotificationAsync(Notification notification);
        Task DeleteNotificationAsync(int notificationId);
    }
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Notification>> GetNotificationsAsync(int userId)
        {
    
            return await _repository.GetNotificationsByUserIdAsync(userId);
        }

        public async Task<Notification> GetNotificationByIdAsync(int notificationId)
        {
            throw new Exception("Not Implimented");
        }

        public async Task<int> AddNotificationAsync(Notification notification)
        {
            return await _repository.AddNotificationAsync(notification);
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            await _repository.UpdateNotificationAsync(notification);
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            await _repository.DeleteNotificationAsync(notificationId);
        }
    }

}
