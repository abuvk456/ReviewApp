using CommonModel;
using Newtonsoft.Json;
using ReviewApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReviewApp.Repository
{
  public interface IWatchlistRepository
  {
    Task<List<WatchlistEntry>> GetWatchlistByUserIdAsync(int userId);
    Task AddToWatchlistAsync(WatchlistEntry entry);
    Task UpdateWatchlistAsync(WatchlistEntry entry);
    Task DeleteFromWatchlistAsync(int userId, int topicId);
  }

  public class WatchlistApiClient : IWatchlistRepository
  {
    private string BaseUrl = ViewModels.Globals.BaseUrl;
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public WatchlistApiClient(string apiKey = "")
    {
      _apiKey = apiKey;
      _httpClient = new HttpClient
      {
        BaseAddress = new Uri(BaseUrl)
      };
    }

    public async Task<List<WatchlistEntry>> GetWatchlistByUserIdAsync(int userId)
    {
      try
      {
        var url = $"/api/watchlist/{userId}";
        var response = await _httpClient.GetAsync(url);
        var results = JsonConvert.DeserializeObject<List<WatchlistEntry>>(await response.Content.ReadAsStringAsync());
        return results;
      }
      catch (Exception ex)
      {
        // Handle exception
        return new List<WatchlistEntry>();
      }
    }

    public async Task AddToWatchlistAsync(WatchlistEntry entry)
    {
      try
      {
        var url = "/api/watchlist";
        var content = new StringContent(JsonConvert.SerializeObject(entry), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
      }
      catch (Exception ex)
      {
        // Handle exception
      }
    }

    public async Task UpdateWatchlistAsync(WatchlistEntry entry)
    {
      try
      {
        var url = $"/api/watchlist/{entry.UserID}/{entry.TopicID}";
        var content = new StringContent(JsonConvert.SerializeObject(entry), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(url, content);
        response.EnsureSuccessStatusCode();
      }
      catch (Exception ex)
      {
        // Handle exception
      }
    }

    public async Task DeleteFromWatchlistAsync(int userId, int topicId)
    {
      try
      {
        var url = $"/api/watchlist/{userId}/{topicId}";
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
