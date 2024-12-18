using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CommonModel;

namespace ReviewApp.Repository
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
        Task<int> AddNotificationAsync(Notification notification);
        Task UpdateNotificationAsync(Notification notification);
        Task DeleteNotificationAsync(int notificationId);
    }
    public class NotificationApiClient : INotificationRepository
    {
        private readonly HttpClient _httpClient;
        private const string _baseUrl = "http://localhost:7073";
        private readonly string _apiKey;

        public NotificationApiClient(string apiKey = "")
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            try
            {
                var url = $"api/notifications/user/{userId}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var notifications = JsonConvert.DeserializeObject<List<Notification>>(result);
                return notifications;
            }
            catch (Exception ex)
             {
                // Handle exception
                return new List<Notification>();
            }
        }

        public async Task<int> AddNotificationAsync(Notification notification)
        {
            try
            {
                string data = JsonConvert.SerializeObject(notification);
                var url = "/api/notifications";
                var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var id = JsonConvert.DeserializeObject<int>(result);
                return id;
            }
            catch (Exception ex)
            {
                // Handle exception
                return -1;
            }
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            try
            {
                string data = JsonConvert.SerializeObject(notification);
                var url = $"/api/notifications/{notification.ID}";
                var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            try
            {
                var url = $"/api/notifications/{notificationId}";
                var response = await _httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }
    }
}
