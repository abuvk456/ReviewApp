using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Data;
using Microsoft.Data.SqlClient;
using ReviewAppAFAPP.Extensions;
using CommonModel;
using System.Data;

namespace ReviewAppAFAPP.Functions
{
  public static class TopicsAPI
  {
    [FunctionName("GetTopics")]
    public static async Task<IActionResult> GetTopics(
     [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Topics")] HttpRequest req,
     ILogger log)
    {
      log.LogInformation("C# HTTP trigger function GetTopics processed a request.");

      try
      {
        DataTable dt = new DataTable();
        string strConString = Environment.GetEnvironmentVariable("SQLConnString");
        using (SqlConnection con = new SqlConnection(strConString))
        {
          con.Open();
          SqlCommand cmd = new SqlCommand("sp_SelectTopics", con);
          cmd.CommandType = CommandType.StoredProcedure;
          int? topicId = null;
          string topicType = null;
          int? createdBy = null;
          string searchTerm = null;
          if (req.Query.TryGetValue("TopicId", out var topicIdStr) && int.TryParse(topicIdStr, out var topicIdVal))
          {
            topicId = topicIdVal;
            cmd.Parameters.AddWithValue("@TopicId", topicId);
          }
          if (req.Query.TryGetValue("TopicType", out var topicTypeVal))
          {
            topicType = topicTypeVal;
            cmd.Parameters.AddWithValue("@TopicType", topicType);
          }
          if (req.Query.TryGetValue("CreatedBy", out var createdByStr) && int.TryParse(createdByStr, out var createdByVal))
          {
            createdBy = createdByVal;
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
          }
          if (req.Query.TryGetValue("SearchTerm", out var searchTermVal))
          {
            searchTerm = searchTermVal;
            cmd.Parameters.AddWithValue("@SearchTerm", searchTerm);
          }
          SqlDataAdapter da = new SqlDataAdapter(cmd);
          da.Fill(dt);
        }

        var data = ModelConvertor.ConvertDataTable<Topic>(dt);
        return new OkObjectResult(data);
      }
      catch (Exception ex)
      {
        return new BadRequestObjectResult(ex.Data);
      }
    }


    [FunctionName("GetTopicById")]
    public static async Task<IActionResult> GetTopicById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Topics/{Id}")] HttpRequest req,
            string Id,
            ILogger log)
    {
      log.LogInformation("C# HTTP trigger function GetTopicById processed a request.");

      try
      {
        DataTable dt = new DataTable();
        string strConString = Environment.GetEnvironmentVariable("SQLConnString");
        using (SqlConnection con = new SqlConnection(strConString))
        {
          con.Open();
          SqlCommand cmd = new SqlCommand($"Select * from Topics where ID='{Id}'", con);
          SqlDataAdapter da = new SqlDataAdapter(cmd);
          da.Fill(dt);
        }

        var data = ModelConvertor.ConvertDataTable<Topic>(dt);
        return new OkObjectResult(data);
      }
      catch (Exception ex)
      {
        return new BadRequestObjectResult(ex.Data);
      }
    }

    [FunctionName("AddTopic")]
    public static async Task<IActionResult> AddTopic(
[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Topics")] HttpRequest req,
ILogger log)
    {
       log.LogInformation("C# HTTP trigger function AddTopic processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var topic = JsonConvert.DeserializeObject<CommonModel.Topic>(requestBody);

                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                int newTopicId;
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_InsertTopic", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Title", topic.Title);
                    cmd.Parameters.AddWithValue("@Description", topic.Description);
                    cmd.Parameters.AddWithValue("@TopicType", topic.TopicType);
                    cmd.Parameters.AddWithValue("@TopicImage", topic.TopicImage);
                    cmd.Parameters.AddWithValue("@TopicVideo", topic.TopicVideo);
                    cmd.Parameters.AddWithValue("@CreatedBy", topic.CreatedBy);
                    cmd.Parameters.AddWithValue("@IMDBRating", topic.IMDBRating);

                    SqlParameter newTopicIdParameter = new SqlParameter("@NewTopicId", SqlDbType.Int);
                    newTopicIdParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(newTopicIdParameter);

                    cmd.ExecuteNonQuery();

                    newTopicId = (int)newTopicIdParameter.Value;
                }

                return new OkObjectResult(newTopicId);
            }
            catch (Exception ex)
            {
                return new OkObjectResult(0);
            }
    }


    [FunctionName("UpdateTopic")]
    public static async Task<IActionResult> UpdateTopic(
    [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Topics")] HttpRequest req,
    ILogger log)
    {
      log.LogInformation("C# HTTP trigger function UpdateTopic processed a request.");

      try
      {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var topic = JsonConvert.DeserializeObject<Topic>(requestBody);

        string strConString = Environment.GetEnvironmentVariable("SQLConnString");
        var cmdResult = 0;
        using (SqlConnection con = new SqlConnection(strConString))
        {
          con.Open();
          SqlCommand cmd = new SqlCommand("sp_UpdateTopics", con);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@topicid", topic.TopicId);
          cmd.Parameters.AddWithValue("@Title", topic.Title);
          cmd.Parameters.AddWithValue("@Description", topic.Description);
          cmd.Parameters.AddWithValue("@TopicType", topic.TopicType);
          cmd.Parameters.AddWithValue("@TopicImage", topic.TopicImage);
          cmd.Parameters.AddWithValue("@TopicVideo", topic.TopicVideo);
          cmd.Parameters.AddWithValue("@CreatedBy", topic.CreatedBy);
          cmd.Parameters.AddWithValue("@IsActive", topic.IsActive.GetHashCode());
          cmd.Parameters.AddWithValue("@IMDBRating", topic.IMDBRating);
          cmdResult = cmd.ExecuteNonQuery();
        }

        return new OkObjectResult(cmdResult);
      }
      catch (Exception ex)
      {
        return new BadRequestObjectResult(ex.Data);
      }
    }


    [FunctionName("DeleteTopic")]
    public static async Task<IActionResult> DeleteTopic(
    [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Topics/{Id}")] HttpRequest req,
    string Id,
    ILogger log)
    {
      log.LogInformation("C# HTTP trigger function DeleteTopic processed a request.");

      try
      {
        string strConString = Environment.GetEnvironmentVariable("SQLConnString");
        var cmdResult = 0;
        using (SqlConnection con = new SqlConnection(strConString))
        {
          con.Open();
          SqlCommand cmd = new SqlCommand("sp_DeleteTopic", con);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@TopicId", Id);
          cmdResult = cmd.ExecuteNonQuery();
        }

        return new OkObjectResult(cmdResult);
      }
      catch (Exception ex)
      {
        return new BadRequestObjectResult(ex.Data);
      }
    }
  }

}
