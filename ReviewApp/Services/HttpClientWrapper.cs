using Newtonsoft.Json.Linq;
using ReviewApp.ViewModels;
using System;
using System.Net.Http.Headers;

namespace ReviewApp.Services
{
  public class HttpWrapper
  {
    private readonly HttpClient _client;

    public HttpWrapper(string baseUrl)
    {
 //     var LVM = MyServiceLocator.Services.GetService<LoginViewModel>();
      _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
      Task.Run(async() => {
        var session = await Helper.GetSessionAsync();
        if (session != null) { 
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);
          }
      });
    }

    public async Task<string> GetAsync(string url)
    {
      HttpResponseMessage response = await _client.GetAsync(url);
      response.EnsureSuccessStatusCode();
      string responseBody = await response.Content.ReadAsStringAsync();
      return responseBody;
    }

    public async Task<string> PostAsync(string url, string data)
    {
      HttpContent content = new StringContent(data);
      HttpResponseMessage response = await _client.PostAsync(url, content);
      response.EnsureSuccessStatusCode();
      string responseBody = await response.Content.ReadAsStringAsync();
      return responseBody;
    }

    public async Task<string> PutAsync(string url, string data)
    {
      HttpContent content = new StringContent(data);
      HttpResponseMessage response = await _client.PutAsync(url, content);
      response.EnsureSuccessStatusCode();
      string responseBody = await response.Content.ReadAsStringAsync();
      return responseBody;
    }

    public async Task<string> DeleteAsync(string url)
    {
      HttpResponseMessage response = await _client.DeleteAsync(url);
      response.EnsureSuccessStatusCode();
      string responseBody = await response.Content.ReadAsStringAsync();
      return responseBody;
    }
  }
}
