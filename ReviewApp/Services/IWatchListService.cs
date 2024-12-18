using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonModel;
using ReviewApp.Repository;

namespace ReviewApp.Services
{
    public interface IWatchlistService
    {
        Task<List<WatchlistEntry>> GetWatchlistByUserIdAsync(int userId);
        Task AddToWatchlistAsync(int userId, int topicId, DateTime watchedDateTime);
        Task UpdateWatchlistAsync(int userId, int topicId, DateTime watchedDateTime);
        Task DeleteFromWatchlistAsync(int userId, int topicId);
    }

    public class WatchlistService : IWatchlistService
    {
        private readonly IWatchlistRepository _watchlistRepository;

        public WatchlistService(IWatchlistRepository watchlistRepository)
        {
            _watchlistRepository = watchlistRepository;
        }

        public Task<List<WatchlistEntry>> GetWatchlistByUserIdAsync(int userId)
        {
            return _watchlistRepository.GetWatchlistByUserIdAsync(userId);
        }

        public Task AddToWatchlistAsync(int userId, int topicId, DateTime watchedDateTime)
        {
            var entry = new WatchlistEntry
            {
                UserID = userId,
                TopicID = topicId,
                WatchedDateTime = watchedDateTime
            };

            return _watchlistRepository.AddToWatchlistAsync(entry);
        }

        public Task UpdateWatchlistAsync(int userId, int topicId, DateTime watchedDateTime)
        {
            var entry = new WatchlistEntry
            {
                UserID = userId,
                TopicID = topicId,
                WatchedDateTime = watchedDateTime
            };

            return _watchlistRepository.UpdateWatchlistAsync(entry);
        }

        public Task DeleteFromWatchlistAsync(int userId, int topicId)
        {
            return _watchlistRepository.DeleteFromWatchlistAsync(userId, topicId);
        }
    }
}
