using Newtonsoft.Json;
using ReviewApp.Model;

namespace ReviewApp.Services
{

    public class TmdbClient
    {
        private const string BaseUrl = "https://api.themoviedb.org";

        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public TmdbClient(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<List<TmdbMovie>> GetPopularMoviesAsync(int page = 1, int pageSize = 20)
        {
            var url = $"/3/movie/popular?api_key={_apiKey}&page={page}&page_size={pageSize}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var results = JsonConvert.DeserializeObject<TmdbSearchResponse<TmdbMovie>>(content);
            return results.Results;
        }
        public async Task<ReviewApp.Model.MovieTMDB> GetMovieDetailsAsync(int movieId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/3/movie/{movieId}?api_key={_apiKey}");
      
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get movie details: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ReviewApp.Model.MovieTMDB>(content);
        }
        public async Task<List<ReviewApp.Model.MovieTMDB>> SearchMoviesByNameAsync(string query, int page = 1, int per_page = 5)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/3/search/movie?api_key={_apiKey}&query={query}&page={page}&page_size={per_page}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to search movies: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();

            var searchResponse = JsonConvert.DeserializeObject<TmdbSearchResponse<MovieTMDB>>(content);

            return searchResponse.Results;
        }

        public async Task<List<TmdbTVShow>> SearchTVShowsByNameAsync(string query, int page=1, int per_page=10)
        {
 

            var request = new HttpRequestMessage(HttpMethod.Get, $"/3/search/tv?api_key={_apiKey}&query={query}&page={page}&page_size={per_page}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to search TV shows: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();

            var searchResponse = JsonConvert.DeserializeObject<TmdbSearchResponse<TmdbTVShow>>(content);

            return searchResponse.Results;
        }
        public async Task<List<MovieVideo>> GetMovieVideosAsync(string movieId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/3/movie/{movieId}/videos?api_key={_apiKey}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get movie videos: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();

            var videosResponse = JsonConvert.DeserializeObject<TmdbVideosResponse<MovieVideo>>(content);

            return videosResponse.Results;
        }
        public async Task<List<MovieVideo>> GetTVShowVideosAsync(string tvShowId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/3/tv/{tvShowId}/videos?api_key={_apiKey}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get TV show videos: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();

            var videosResponse = JsonConvert.DeserializeObject<TmdbVideosResponse<MovieVideo>>(content);

            return videosResponse.Results;
        }
        public async Task<string> GetMovieTrailerUrlAsync(string id, bool isTV=false)
        {
            string key = "";
            if (isTV)
            {
                // Get the videos for the movie
                var videos = await GetTVShowVideosAsync(id);

                // Find the first video that is a trailer
                //var trailer = videos.FirstOrDefault(v => v.Type.ToLower() == "trailer");
                // if (trailer == null)
                //{
                // Trailer not found
                //  return null;
                // }
                key = videos.FirstOrDefault()?.Key;
            }
            else
            {
                // Get the videos for the movie
                var videos = await GetMovieVideosAsync(id);

                // Find the first video that is a trailer
                key = videos.FirstOrDefault()?.Key;
            }

            // Build the YouTube URL for the trailer
            return $"https://www.youtube.com/watch?v={key}";
        }

    }
    public class TmdbSearchResponse<T>
    {
        [JsonProperty("results")]
        public List<T> Results { get; set; }
    }
    public class TmdbVideosResponse<T> where T : MovieVideo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("results")]
        public List<T> Results { get; set; }
    }
    public class TmdbMovie
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("release_date")]
        public DateTime? ReleaseDate { get; set; }

        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }
       

        public string GetFullPosterUrl(string size = "w500")
        {
            return $"https://image.tmdb.org/t/p/{size}/{PosterPath}";
        }
    }
    
public class TMDBHelper
    {
       public TmdbClient TmdbClient;
        public string apiKey = "c2c8e1d066228fae7d755e8249cbdae7";
        public TMDBHelper()
        {
            TmdbClient = new TmdbClient(apiKey);


        }
        
    }
}
