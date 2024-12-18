using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommonModel;
using Newtonsoft.Json;
using ReviewApp.Repository;

namespace ReviewApp.Services
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        Task<List<User>> LoadRecomendations(int userid);
        Task<int> CreateUserAsync(User user, string password);
        Task<bool> VerifyPasswordAsync(User user, string password);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> FollowUserAsync(int followerId, int followeeId);
        Task<bool> UnfollowUserAsync(int followerId, int followeeId);
        Task<bool> VoteUser(Vote vote);
        Task<User> LoginUserAsync(string email, string password);
        Task<int> SignUpUserAsync(User user, string password);
        Task<UserProfileInfo> GetUserProfileInfo(int UserId);
        Task<List<User>> GetUserFollowings(int id);
        Task<bool> ResetPassword(string username, string password);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userApiClient;

        public UserService()
        {

            _userApiClient = new UserApiClient(); ;
        }
        public async Task<bool> ResetPassword(string username, string password)
        {
            return await _userApiClient.ResetPassword(username, password);
        }
        public Task<int> CreateUserAsync(User user, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FollowUserAsync(int followerId, int followeeId)
        {
           return _userApiClient.FollowUserAsync(followerId, followeeId);
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            return _userApiClient.GetByEmailAsync(email);
        }

        public Task<User> GetUserByIdAsync(int id)
        {
            return _userApiClient.GetByIdAsync(id);
        }
        public Task<UserProfileInfo> GetUserProfileInfo(int id)
        {
            return _userApiClient.GetUserStats(id);
        }

        public Task<User> GetUserByUsernameAsync(string username)
        {
            return _userApiClient.GetByUsernameAsync(username);
        }

        public Task<User> LoginUserAsync(string email, string password)
        {
            return _userApiClient.Login(new CommonModel.User { Email = email, Password = password });
        }

        public Task<List<User>> LoadRecomendations(int userid)
        {
            return _userApiClient.LoadRecomendations(userid);
        }

        public Task<int> SignUpUserAsync(User user, string password)
        {
            //todo Call Vote user from Signup Page
            return _userApiClient.AddAsync(user);
        }

        public Task<bool> UnfollowUserAsync(int followerId, int followeeId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VerifyPasswordAsync(User user, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> VoteUser(Vote vote)
        {
            //todo Call Vote user from User Detail Page
            return await _userApiClient.VoteAsync(vote) > 0;
        }
       public async Task<List<User>> GetUserFollowings(int id)
        {
            return await _userApiClient.GetUserFollowings(id);
        }
    }
}
