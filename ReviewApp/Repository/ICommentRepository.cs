using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonModel;
using Newtonsoft.Json;
using ReviewApp.Services;

namespace ReviewApp.Repository
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetCommentsAsync(int? commentId = null, int? commentedBy = null, int? topicId = null);
        Task<int> AddCommentAsync(Comment comment);
        Task UpdateCommentAsync(int commentId, Comment comment);
        Task DeleteCommentAsync(int commentId);
    }

    public class CommentApiClient : ICommentRepository
    {
        private string BaseUrl = ViewModels.Globals.BaseUrl;
        private readonly string _apiKey;
        private readonly HttpWrapper _httpClient;

        public CommentApiClient(string apiKey = "")
        {
            _apiKey = apiKey;
            _httpClient = new HttpWrapper(BaseUrl);
        }

        public async Task<List<Comment>> GetCommentsAsync(int? commentId = null, int? commentedBy = null, int? topicId = null)
        {
            try
            {
                string url = "/api/Comments?";
                if (commentId.HasValue)
                    url += $"CommentId={commentId.Value}&";
                if (commentedBy.HasValue)
                    url += $"CommentedBy={commentedBy.Value}&";
                if (topicId.HasValue)
                    url += $"TopicId={topicId.Value}";

                var response = await _httpClient.GetAsync(url);
                var results = JsonConvert.DeserializeObject<List<Comment>>(response);
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> AddCommentAsync(Comment comment)
        {
            try
            {
                string data = JsonConvert.SerializeObject(comment);
                var url = "/api/Comments";
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

        public async Task UpdateCommentAsync(int commentId, Comment comment)
        {
            try
            {
                string data = JsonConvert.SerializeObject(comment);
                var url = $"/api/Comments/{commentId}";
                await _httpClient.PutAsync(url, data);
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            try
            {
                var url = $"/api/Comments/{commentId}";
                await _httpClient.DeleteAsync(url);
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }
    }
}
