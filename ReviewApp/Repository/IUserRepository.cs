using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModel;
using Newtonsoft.Json;
using ReviewApp.Services;
using ReviewApp.ViewModels;

namespace ReviewApp.Repository
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int userId);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<int> AddAsync(User user);
        Task UpdateAsync(User user);
        //Task DeleteAsync(int userId);
        Task<User> Login(User user);
        Task<int> VoteAsync(Vote vote);
        Task<UserProfileInfo> GetUserStats(int id);
        Task<List<User>> LoadRecomendations(int userid);
        Task<bool> FollowUserAsync(int followerId, int followeeId);
        Task<List<User>> GetUserFollowings(int id);
        Task<bool> ResetPassword(string username, string password);
    }
    public class UserApiClient : IUserRepository
    {
        private string BaseUrl = ViewModels.Globals.BaseUrl;
        private readonly string _apiKey;
        private readonly HttpWrapper _httpClient;

        public UserApiClient(string apiKey = "")
        {
            _apiKey = apiKey;
            _httpClient = new HttpWrapper(BaseUrl);
        }
        public async Task<User> Login(User user)
        {
            try
            {
                string data = JsonConvert.SerializeObject(new { Email = user.Email, Password = user.Password });
                var url = "/api/Users/SignIn";
                var response = await _httpClient.PostAsync(url, data);
                var results = JsonConvert.DeserializeObject<User>(response);
                return results;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<List<User>> LoadRecomendations(int UserID)
        {
            try
            {
                var url = $"/api/Userswithsamewatchlist?UserID={UserID}";
                var response = await _httpClient.GetAsync(url);
                var results = JsonConvert.DeserializeObject<List<User>>(response);
                return results;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<List<User>> GetUserFollowings(int UserID)
        {
            try
            {
                var url = $"/api/Followings/{UserID}";
                var response = await _httpClient.GetAsync(url);
                var results = JsonConvert.DeserializeObject<List<User>>(response);
                return results;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<bool> FollowUserAsync(int followerId, int followeeId)
        {
            try
            {
                var url = $"/api/followings/{followerId}/{followeeId}";
                var response = await _httpClient.GetAsync(url);
                var results = JsonConvert.DeserializeObject<int>(response);
                return results > 0;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        public async Task<User> GetByIdAsync(int userId)
        {
            try
            {
                var url = $"/api/Users/{userId}";
                var response = await _httpClient.GetAsync(url);
                var result = JsonConvert.DeserializeObject<User>(response);
                return result;
            }
            catch (Exception ex)
            {
                // Handle exception
                return null;
            }
        }
        public async Task<UserProfileInfo> GetUserStats(int userId)
        {
            try
            {
                var url = $"/api/UserStats/{userId}";
                var response = await _httpClient.GetAsync(url);
                var result = JsonConvert.DeserializeObject<UserProfileInfo>(response);
                return result;
            }
            catch (Exception ex)
            {
                // Handle exception
                return new UserProfileInfo();
            }
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            try
            {
                var url = $"/api/Users/GetByUsername/{username}";
                var response = await _httpClient.GetAsync(url);
                var result = JsonConvert.DeserializeObject<User>(response);
                return result;
            }
            catch (Exception ex)
            {
                // Handle exception
                return null;
            }
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            try
            {
                var url = $"/api/Users/GetByEmail/{email}";
                var response = await _httpClient.GetAsync(url);
                var result = JsonConvert.DeserializeObject<User>(response);
                return result;
            }
            catch (Exception ex)
            {
                // Handle exception
                return null;
            }
        }

        public async Task<int> AddAsync(User user)
        {
            try
            {
                string data = JsonConvert.SerializeObject(user);
                var url = "/api/Users";
                var response = await _httpClient.PostAsync(url, data);
                var results = JsonConvert.DeserializeObject<int>(response);
                return results;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<bool> ResetPassword(string username, string password)
        {
            try
            {
                string data = JsonConvert.SerializeObject(new { Email = username, NewPassword = password });
                var url = "/api/ResetPassword";
                var response = await _httpClient.PostAsync(url, data);
                var results = JsonConvert.DeserializeObject<int>(response);
                return results > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task UpdateAsync(User user)
        {
            try
            {
                string data = JsonConvert.SerializeObject(user);
                var url = $"/api/Users/{user.UserId}";
                var response = await _httpClient.PutAsync(url, data);
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }

        public async Task DeleteAsync(int userId)
        {
            try
            {
                var url = $"/api/Users/{userId}";
                var response = await _httpClient.DeleteAsync(url);
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }
        public async Task<int> VoteAsync(Vote vote)
        {
            try
            {
                string data = JsonConvert.SerializeObject(vote);
                var url = "/api/Votes";
                var response = await _httpClient.PostAsync(url, data);
                var results = JsonConvert.DeserializeObject<int>(response);
                return results;
            }
            catch (Exception ex)
            {
                return 0;
                // Handle exception
            }
        }
    }


}
