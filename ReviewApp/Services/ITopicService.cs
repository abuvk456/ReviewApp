using System.Collections.Generic;
using System.Threading.Tasks;
using CommonModel;
using ReviewApp.Repository;

namespace ReviewApp.Services
{
    public interface ITopicService
    {
        Task<List<Topic>> GetTopicsAsync();
        Task<Topic> GetTopicByIdAsync(int topicId);
        Task<int> AddTopicAsync(Topic topic);
        Task UpdateTopicAsync(Topic topic);
        Task DeleteTopicAsync(int topicId);
        Task<List<Topic>> GetAllTopics(TopicType type = TopicType.Movie, int userId = 0, int topicId = 0, string searchTerm = "");

    }

    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _topicRepository;
        public async Task<List<Topic>> GetAllTopics(TopicType type = TopicType.Movie, int userId = 0, int topicId = 0, string searchTerm = "")
        {
            try
            {
                return await _topicRepository.GetTopicsAsync(type, userId, topicId, searchTerm);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public TopicService(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        public async Task<List<Topic>> GetTopicsAsync()
        {
            return await _topicRepository.GetTopicsAsync();
        }

        public async Task<Topic> GetTopicByIdAsync(int topicId)
        {
            return await _topicRepository.GetTopicByIdAsync(topicId);
        }

        public async Task<int> AddTopicAsync(Topic topic)
        {
            return await _topicRepository.AddTopicAsync(topic);
        }

        public async Task UpdateTopicAsync(Topic topic)
        {
            await _topicRepository.UpdateTopicAsync(topic);
        }

        public async Task DeleteTopicAsync(int topicId)
        {
            await _topicRepository.DeleteTopicAsync(topicId);
        }
    }
}
