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
    public static class CommentsApi
    {
        [FunctionName("GetComments")]
        public static async Task<IActionResult> GetComments(
         [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Comments")] HttpRequest req,
         ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetComments processed a request.");

            try
            {
                DataTable dt = new DataTable();
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_ReadComment", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    int? commentId = null;
                    int? commentedBy = null;
                    int? topicId = null;
                    if (req.Query.TryGetValue("CommentId", out var commentIdStr) && int.TryParse(commentIdStr, out var commentIdVal))
                    {
                        commentId = commentIdVal;
                        cmd.Parameters.AddWithValue("@CommentId", commentId);
                    }
                    if (req.Query.TryGetValue("CommentedBy", out var commentedByStr) && int.TryParse(commentedByStr, out var commentedByVal))
                    {
                        commentedBy = commentedByVal;
                        cmd.Parameters.AddWithValue("@CommentedBy", commentedBy);
                    }
                    if (req.Query.TryGetValue("TopicId", out var topicIdStr) && int.TryParse(topicIdStr, out var topicIdVal))
                    {
                        topicId = topicIdVal;
                        cmd.Parameters.AddWithValue("@TopicId", topicId);
                    }
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                var data = ModelConvertor.ConvertDataTable<Comment>(dt);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }
        [FunctionName("CreateComment")]
        public static async Task<IActionResult> CreateComment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Comments")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function CreateComment processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var comment = JsonConvert.DeserializeObject<Comment>(requestBody);

                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {//openining the connection
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_CreateComment", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TopicId", comment.TopicId);
                    cmd.Parameters.AddWithValue("@CommentText", comment.CommentText);
                    cmd.Parameters.AddWithValue("@CommentedBy", comment.CommentedBy);
                    var idParam = new SqlParameter("@CommentId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idParam);
                    cmd.ExecuteNonQuery();
                    comment.CommentId = (int)idParam.Value;
                }

                return new OkObjectResult(comment);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }
        [FunctionName("UpdateComment")]
        public static async Task<IActionResult> UpdateComment(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Comments/{commentId}")] HttpRequest req,
        int commentId,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function UpdateComment processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var comment = JsonConvert.DeserializeObject<Comment>(requestBody);

                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_UpdateComment", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CommentId", commentId);
                    cmd.Parameters.AddWithValue("@CommentText", comment.CommentText);
                    cmd.Parameters.AddWithValue("@IsDeleted", comment.IsDeleted);
                    cmd.ExecuteNonQuery();
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }
        [FunctionName("DeleteComment")]
        public static async Task<IActionResult> DeleteComment(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Comments/{commentId}")] HttpRequest req,
        int commentId,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function DeleteMessage processed a request.");

            try
            {
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_DeleteComment", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CommentId", commentId);
                    cmd.ExecuteNonQuery();
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }
    }
}




