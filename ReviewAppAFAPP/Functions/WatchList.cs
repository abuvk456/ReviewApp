using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ReviewAppAFAPP.Extensions;
using System.Data.SqlClient;
using System.Data;
using CommonModel;
using System.Collections.Generic;

namespace ReviewAppAFAPP.Functions
{
  public static class WatchlistApi
  {
    [FunctionName("GetWatchlistByUserID")]
    public static async Task<IActionResult> GetWatchlistByUserID(
     [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "watchlist/{userID}")] HttpRequest req,
     ILogger log, string userID)
    {
      log.LogInformation("C# HTTP trigger function GetWatchlistByUserID processed a request.");

      try
      {
        if (int.TryParse(userID, out int parsedUserID))
        {
          DataTable dt = new DataTable();
          string strConString = Environment.GetEnvironmentVariable("SQLConnString");
          using (SqlConnection con = new SqlConnection(strConString))
          {
            con.Open();
            SqlCommand cmd = new SqlCommand("SelectWatchlistByUserID", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", parsedUserID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
          }
          var data = ModelConvertor.ConvertDataTable<Topic>(dt);
          var watchlist = new List<WatchlistEntry>();
          foreach (var item in data)
          {
            var entry = new WatchlistEntry
            {
              UserID = item.CreatedBy,
              TopicID = item.TopicId,

              Movie = new Topic
              {
                TopicId = item.TopicId,
                Title = item.Title,
                Description = item.Description,
                TopicType = item.TopicType,
                TopicImage = item.TopicImage,
                TopicVideo = item.TopicVideo,
                IMDBRating = item.IMDBRating,
                IDMBID = item.IDMBID,
                TMDBID = item.TMDBID,
                CreatedBy = item.CreatedBy,
                CreatedDate = item.CreatedDate

              }
            };
            watchlist.Add(entry);
          }

          return new OkObjectResult(watchlist);
        }
        else
        {
          return new BadRequestObjectResult("Invalid user ID.");
        }
      }
      catch (Exception ex)
      {
        return new BadRequestObjectResult(ex.Data);
      }
    }


    [FunctionName("AddToWatchlist")]
    public static async Task<IActionResult> AddToWatchlist(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "watchlist")] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("AddToWatchlist HTTP trigger function processed a request.");
      try
      {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var entry = JsonConvert.DeserializeObject<WatchlistEntry>(requestBody);

        using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQLConnString")))
        {
          connection.Open();

          using (SqlCommand command = new SqlCommand("InsertWatchlist", connection))
          {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserID", entry.UserID);
            command.Parameters.AddWithValue("@TopicID", entry.TopicID);
            command.Parameters.AddWithValue("@WatchedDateTime", DateTime.UtcNow);

            await command.ExecuteNonQueryAsync();
          }
        }

        return new OkResult();
      }
      catch (Exception ex)
      {
        return new BadRequestObjectResult(ex.Data);
      }
    }

    [FunctionName("UpdateWatchlist")]
    public static async Task<IActionResult> UpdateWatchlist(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "watchlist")] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("UpdateWatchlist HTTP trigger function processed a request.");
      try
      {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var entry = JsonConvert.DeserializeObject<WatchlistEntry>(requestBody);

        using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQLConnString")))
        {
          connection.Open();

          using (SqlCommand command = new SqlCommand("UpdateWatchlist", connection))
          {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserID", entry.UserID);
            command.Parameters.AddWithValue("@TopicID", entry.TopicID);
            command.Parameters.AddWithValue("@WatchedDateTime", DateTime.UtcNow);

            await command.ExecuteNonQueryAsync();
          }
        }

        return new OkResult();
      }
      catch (Exception ex)
      {
        return new BadRequestObjectResult(ex.Data);
      }
    }

    [FunctionName("DeleteFromWatchlist")]
    public static async Task<IActionResult> DeleteFromWatchlist(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "watchlist/{userID}/{topicID}")] HttpRequest req,
        ILogger log, string userID, string topicID)
    {
      log.LogInformation("DeleteFromWatchlist HTTP trigger function processed a request.");

      try
      {
        if (int.TryParse(userID, out int parsedUserID) && int.TryParse(topicID, out int parsedTopicID))
        {
          using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQLConnString")))
          {
            connection.Open();

            using (SqlCommand command = new SqlCommand("DeleteWatchlist", connection))
            {
              command.CommandType = CommandType.StoredProcedure;
              command.Parameters.AddWithValue("@UserID", parsedUserID);
              command.Parameters.AddWithValue("@TopicID", parsedTopicID);

              await command.ExecuteNonQueryAsync();
            }
          }

          return new OkResult();
        }
        else
        {
          return new BadRequestObjectResult("Invalid user ID or topic ID.");
        }
      }
      catch (Exception ex)
      {
        return new BadRequestObjectResult(ex.Data);
      }
    }
  }
}
