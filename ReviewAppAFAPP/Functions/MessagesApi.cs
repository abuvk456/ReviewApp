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

namespace ReviewAppAFAPP.Functions
{
    public static class MessagesApi
    {
        [FunctionName("GetMessages")]
        public static async Task<IActionResult> GetMessages(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "messages")] HttpRequest req,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetMessages processed a request.");

            try
            {
                int? sentBy = null;
                int? sentTo = null;

                // Read optional query string parameters
                if (req.Query.ContainsKey("sentBy") && int.TryParse(req.Query["sentBy"], out int parsedSentBy))
                {
                    sentBy = parsedSentBy;
                }

                if (req.Query.ContainsKey("sentTo") && int.TryParse(req.Query["sentTo"], out int parsedSentTo))
                {
                    sentTo = parsedSentTo;
                }

                DataTable dt = new DataTable();
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_GetMessagesByUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (sentBy.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@UserID", sentBy.Value);
                    }

                    if (sentTo.HasValue)
                    {
                      //  cmd.Parameters.AddWithValue("@SentTo", sentTo.Value);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                var data = ModelConvertor.ConvertDataTable<Message>(dt);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("GetMessageById")]
        public static async Task<IActionResult> GetMessageById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "messages/{id}")] HttpRequest req, int id,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function GetMessageById processed a request. Id: {id}");

            try
            {
                DataTable dt = new DataTable();
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_GetMessageById", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MessageID", id);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                var data = ModelConvertor.ConvertDataTable<Message>(dt);
                if (data.Count == 0)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(data[0]);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
        [FunctionName("CreateMessage")]
        public static async Task<IActionResult> CreateMessage(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "messages")] HttpRequest req,
    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function CreateMessage processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var message = JsonConvert.DeserializeObject<Message>(requestBody);

                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_InsertMessage", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@SentBy", message.SentBy);
                    cmd.Parameters.AddWithValue("@SentTo", message.SentTo);
                    cmd.Parameters.AddWithValue("@SentDatetime", message.SentDatetime);
                    cmd.Parameters.AddWithValue("@MessageText", message.MessageText);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return new OkObjectResult(message);
                    }
                    else
                    {
                        return new BadRequestObjectResult("Failed to create message");
                    }
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }
        [FunctionName("DeleteMessage")]
        public static async Task<IActionResult> DeleteMessage(
    [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "messages/{id}")] HttpRequest req,
    ILogger log, int id)
        {
            log.LogInformation("C# HTTP trigger function DeleteMessage processed a request.");

            try
            {
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_DeleteMessage", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MessageID", id);
                    cmd.ExecuteNonQuery();
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }
        [FunctionName("GetConversationByUsers")]
        public static async Task<IActionResult> GetConversationByUsers(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "conversation/{user1Id}/{user2Id}")] HttpRequest req,
    ILogger log, string user1Id, string user2Id)
        {
            log.LogInformation("C# HTTP trigger function GetConversationByUsers processed a request.");

            try
            {
                DataTable dt = new DataTable();
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_GetConversationByUsers", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@User1", user1Id);
                    cmd.Parameters.AddWithValue("@User2", user2Id);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                var data = ModelConvertor.ConvertDataTable<Message>(dt);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }

    }
}
