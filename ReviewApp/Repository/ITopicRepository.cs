using Newtonsoft.Json;
using ReviewApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonModel;

namespace ReviewApp.Repository
{
    public interface ITopicRepository
    {
        Task<List<Topic>> GetTopicsAsync();
        Task<Topic> GetTopicByIdAsync(int topicId);
        Task<int> AddTopicAsync(Topic topic);
        Task UpdateTopicAsync(Topic topic);
        Task DeleteTopicAsync(int topicId);
        Task<List<Topic>> GetTopicsAsync(TopicType type = TopicType.Movie, int userId = 0, int topicId = 0, string searchTerm = "");
    }

    public class TopicApiClient : ITopicRepository
    {
        private string BaseUrl = ViewModels.Globals.BaseUrl;
        private readonly string _apiKey;
        private readonly HttpWrapper _httpClient;
        public async Task<List<Topic>> GetTopicsAsync(TopicType type = TopicType.Movie, int userId = 0, int topicId = 0, string searchTerm = "")
        {
            try
            {
                string url = "/api/Topics?";
                if (type != TopicType.All)
                    url += $"TopicType={type.ToString()}";
                if (userId > 0)
                {
                    url += $"&CreatedBy={userId}";
                }
                if (topicId > 0)
                {
                    url += $"&TopicId={topicId}";
                }
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    url += $"&SearchTerm={searchTerm}";
                }

                var response = await _httpClient.GetAsync(url);
                var results = JsonConvert.DeserializeObject<List<Topic>>(response);
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public TopicApiClient(string apiKey = "")
        {
            _apiKey = apiKey;
            _httpClient = new HttpWrapper(BaseUrl);
        }

        public async Task<List<Topic>> GetTopicsAsync()
        {
            try
            {
                var url = "/api/Topics";
                var response = await _httpClient.GetAsync(url);
                var results = JsonConvert.DeserializeObject<List<Topic>>(response);
                return results;
            }
            catch (Exception ex)
            {
                // Handle exception
                return new List<Topic>();
            }
        }

        public async Task<Topic> GetTopicByIdAsync(int topicId)
        {
            try
            {
                var url = $"/api/Topics/{topicId}";
                var response = await _httpClient.GetAsync(url);
                var result = JsonConvert.DeserializeObject<Topic>(response);
                return result;
            }
            catch (Exception ex)
            {
                // Handle exception
                return null;
            }
        }

        public async Task<int> AddTopicAsync(Topic topic)
        {
            try
            {
                string data = JsonConvert.SerializeObject(topic);
                var url = "/api/Topics";
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

        public async Task UpdateTopicAsync(Topic topic)
        {
            try
            {
                string data = JsonConvert.SerializeObject(topic);
                var url = $"/api/Topics/{topic.TopicId}";
                await _httpClient.PutAsync(url, data);
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }

        public async Task DeleteTopicAsync(int topicId)
        {
            try
            {
                var url = $"/api/Topics/{topicId}";
                await _httpClient.DeleteAsync(url);
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }
    }
}
