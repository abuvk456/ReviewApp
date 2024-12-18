using CommonModel;
using Newtonsoft.Json;
using ReviewApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewApp.Repository
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetMessagesAsync();
        Task<List<Message>> GetMessagesByUserIdAsync(int userId);
        Task<List<Message>> GetConverstationAsync(int userId, int userid2);
        Task<Message> GetMessageByIdAsync(int messageId);
        Task<int> AddMessageAsync(Message message);
        Task UpdateMessageAsync(Message message);
        Task DeleteMessageAsync(int messageId);
    }
    public class MessageApiClient : IMessageRepository
    {
        private string BaseUrl = ViewModels.Globals.BaseUrl;
        private readonly string _apiKey;
        private readonly HttpWrapper _httpClient;

        public MessageApiClient(string apiKey = "")
        {
            _apiKey = apiKey;
            _httpClient = new HttpWrapper(BaseUrl);
        }

        public async Task<List<Message>> GetMessagesAsync()
        {
            try
            {
                var url = "/api/Messages";
                var response = await _httpClient.GetAsync(url);
                var results = JsonConvert.DeserializeObject<List<Message>>(response);
                return results;
            }
            catch (Exception ex)
            {
                // Handle exception
                return new List<Message>();
            }
        }

        public async Task<List<Message>> GetMessagesByUserIdAsync(int userId)
        {
            try
            {
                var url = $"/api/Messages?sentby={userId}";
                var response = await _httpClient.GetAsync(url);
                var results = JsonConvert.DeserializeObject<List<Message>>(response);
                return results;
            }
            catch (Exception ex)
            {
                // Handle exception
                return new List<Message>();
            }
        }
        public async Task<List<Message>> GetConverstationAsync(int userId, int userid2)
        {
            try
            {
                var url = $"/api/conversation/{userId}/{userid2}";
                var response = await _httpClient.GetAsync(url);
                var results = JsonConvert.DeserializeObject<List<Message>>(response);
                return results;
            }
            catch (Exception ex)
            {
                // Handle exception
                return new List<Message>();
            }
        }

        public async Task<int> AddMessageAsync(Message message)
        {
            try
            {
                string data = JsonConvert.SerializeObject(message);
                var url = "/api/Messages";
                var response = await _httpClient.PostAsync(url, data);
                var result = JsonConvert.DeserializeObject<int>(response);
                return result;
            }
            catch (Exception ex)
            {
                // Handle exception
            }
            return 0;
        }

        public Task<Message> GetMessageByIdAsync(int messageId)
        {
            throw new NotImplementedException();
        }

      

        public Task UpdateMessageAsync(Message message)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMessageAsync(int messageId)
        {
            throw new NotImplementedException();
        }
    }
}
