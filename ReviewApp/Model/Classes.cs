using System;
using Newtonsoft.Json;
using CommonModel;

namespace ReviewApp.Model
{
    public class MovieTMDB
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("release_date")]
        public DateTime? ReleaseDate { get; set; }

        [JsonProperty("vote_average")]
        public double? VoteAverage { get; set; }

        [JsonProperty("vote_count")]
        public int? VoteCount { get; set; }

        [JsonProperty("genres")]
        public List<Genre> Genres { get; set; }

        [JsonProperty("imdb_id")]
        public string IMDbId { get; set; }

        [JsonProperty("imdb_rating")]
        public double IMDbRating { get; set; }

        [JsonProperty("credits")]
        public Credits Credits { get; set; }

        public string GetFullPosterUrl(string size = "w500")
        {
            return $"https://image.tmdb.org/t/p/{size}/{PosterPath}";
        }
        public string ImageUrl
        {
            get
            {
                return GetFullPosterUrl();
            }
        }
    }

    public class Credits
    {
        [JsonProperty("cast")]
        public List<Cast> Cast { get; set; }

        [JsonProperty("crew")]
        public List<Crew> Crew { get; set; }

        //[JsonIgnore]
        //public List<Cast> MainCast
        //{
        //    get
        //    {
        //        // Retrieve the credits for the movie
        //        var client = new HttpClient();
        //        var apiKey = "YOUR_API_KEY";
        //        var url = $"https://api.themoviedb.org/3/movie/{MovieId}/credits?api_key={apiKey}";
        //        var response = client.GetAsync(url).Result;
        //        var credits = JsonConvert.DeserializeObject<Credits>(response.Content.ReadAsStringAsync().Result);

        //        // Filter the cast list to include only the main cast members
        //        var mainCast = credits.Cast.Where(c => c.Order <= 5).ToList();

        //        return mainCast;
        //    }
        //}
    }


    public class Cast
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("character")]
        public string Character { get; set; }

        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; }
    }

    public class Crew
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("job")]
        public string Job { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; }
    }

    public class Genre
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
    public class TVShow
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
    }

    public class Movie
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public int Rating { get; set; }
        public string Genre { get; set; }
    }

    public class Other
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
    }

    public class TopicDD
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TopicType Type { get; set; } = TopicType.Movie;
        public string TopicImage { get; set; }
        public string TopicVideo { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string IDMBID { get; set; }
        public string TMDBID { get; set; }
        public int IMDBRating { get; set; }
  }

  
    public class TmdbTVShow
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("first_air_date")]
        public DateTime? FirstAirDate { get; set; }

        [JsonProperty("last_air_date")]
        public DateTime? LastAirDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("vote_average")]
        public float VoteAverage { get; set; }

        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }

        [JsonProperty("genres")]
        public List<Genre> Genres { get; set; }

        [JsonProperty("created_by")]
        public List<TmdbPerson> CreatedBy { get; set; }

        [JsonProperty("seasons")]
        public List<TmdbSeason> Seasons { get; set; }

        [JsonProperty("episode_run_time")]
        public List<int> EpisodeRunTime { get; set; }
        public string GetFullPosterUrl(string size = "w500")
        {
            return $"https://image.tmdb.org/t/p/{size}/{PosterPath}";
        }
        public string ImageUrl
        {
            get
            {
                return GetFullPosterUrl();
            }
        }
    }
    public class TmdbPerson
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; }
    }

    public class TmdbSeason
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("season_number")]
        public int SeasonNumber { get; set; }

        [JsonProperty("episode_count")]
        public int EpisodeCount { get; set; }

        [JsonProperty("air_date")]
        public DateTime? AirDate { get; set; }
    }
    public class MovieVideo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("site")]
        public string Site { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }


}
