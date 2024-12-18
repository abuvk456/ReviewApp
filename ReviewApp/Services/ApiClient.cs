using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModel;
using ReviewApp.Services;
namespace ReviewApp.Services
{
  public class ApiClient 
  {
   
   // private const string BaseUrl = "https://reviewapp101.azurewebsites.net";
    private string BaseUrl = ViewModels.Globals.BaseUrl;
    private readonly string _apiKey;
    private readonly HttpWrapper _httpClient;

    public ApiClient(string apiKey = "")
    {
      _apiKey = apiKey;
      _httpClient = new HttpWrapper(BaseUrl);
      

    }
    public async Task<User> Login(User user)
    {
      try
      {
        string data = JsonConvert.SerializeObject(new {Email=user.Email,Password=user.Password });
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
    public async Task<int> AddUser(User user)
    {
      try
      {
        string data = JsonConvert.SerializeObject(user);
        var url = "/api/Users";
        var response = await _httpClient.PostAsync(url, data);
        var results = JsonConvert.DeserializeObject<int>(response);
        return results;
      }
      catch(Exception ex){
        return 0;
      }
    }
    public async Task<object> AddTopic(Topic topic)
    {
      try
      {
        string data = JsonConvert.SerializeObject(topic);
        var url = "/api/Topics";
        var response = await _httpClient.PostAsync(url, data);
        var results = JsonConvert.DeserializeObject<int>(response);
        return results;
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }
    public async Task<List<Topic>> GetAllTopics()
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
        throw new Exception(ex.Message);
      }
    }
    public async Task<List<Topic>> GetAllTopics(TopicType type=TopicType.Movie,int userid=0,int topicid=0, string SearchTerm="")
    {
      try
      {
        string url = $"/api/Topics?";
        if (type != TopicType.All)
        url+= $"TopicType={type.ToString()}";
        if(userid>0)
        {
          url += $"&CreatedBy={userid}";
        }
        if(topicid>0)
        {
          url += $"&TopicId={topicid}";
        }
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
          url += $"&searchTerm={SearchTerm}";
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
    public async Task<bool> DeleteTopic(string topicID)
    {
      try
      {
        var url = @"/api/Topics/" + topicID;
        var response = await _httpClient.DeleteAsync(url);
        var results = JsonConvert.DeserializeObject<int>(response);
        return results > 0;
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
      public async Task<bool> UpdateTopic(Topic topic)
      {
        try
        {
          string data = JsonConvert.SerializeObject(topic);
          var url = "/api/Topics";
          var response = await _httpClient.PutAsync(url, data);
          var results = JsonConvert.DeserializeObject<int>(response);
          return results>0;
        }
        catch (Exception ex)
        {
          return false;
        }
      
    }

    public async Task<List<Comment>> GetComments(int topicid)
    {

      try
      {
        
        var url = $"/api/Comments?TopicId={topicid}";
        var response = await _httpClient.GetAsync(url);
        var results = JsonConvert.DeserializeObject<List<Comment>>(response);
        return results;
      }
      catch (Exception ex)
      {
        return new List<Comment>();
      }
    }

    public async Task<int> AddComments(Comment comment)
    {
      try
      {
        string data = JsonConvert.SerializeObject(comment);
        var url = "/api/Comments";
        var response = await _httpClient.PostAsync(url, data);
        var results = JsonConvert.DeserializeObject<int>(response);
        return results;
      }
      catch (Exception ex)
      {
        return 0;
      }

    }
  }
}
