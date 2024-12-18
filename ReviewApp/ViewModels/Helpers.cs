using CommonModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewApp.ViewModels
{
  public class Helper
  {

    public async static void SetSessionAsync(Session SessionInfo)
    {
      await SecureStorage.SetAsync("Token", SessionInfo.Token);
      await SecureStorage.SetAsync("TokenExpirationDate", SessionInfo.ExpirationDate.ToString());
      await SecureStorage.SetAsync("UserId", SessionInfo.UserId.ToString());
      await SecureStorage.SetAsync("SessionId", SessionInfo.SessionId.ToString());
    }

    public static async Task<Session> GetSessionAsync()
    {
      string token = await SecureStorage.GetAsync("Token");
      string tokenExpirationDate = await SecureStorage.GetAsync("TokenExpirationDate");
      string userId = await SecureStorage.GetAsync("UserId");

      if (string.IsNullOrWhiteSpace(token))
        return null;

      DateTime ExDate;
      if (DateTime.TryParse(tokenExpirationDate, out ExDate))
      {
        if (DateTime.UtcNow > ExDate)
        {
          // Token has expired
          ClearSession();
          return null;
        }
      }

      int userIdValue;
      if (!int.TryParse(userId, out userIdValue))
      {
        // Failed to parse user ID
         ClearSession();
        return null;
      }

      return new Session
      {
        Token = token,
        UserId = userIdValue,
        ExpirationDate = ExDate
      };
    }
    public async static void ClearSession()
    {
      SecureStorage.RemoveAll();
      
    }

  }

}
