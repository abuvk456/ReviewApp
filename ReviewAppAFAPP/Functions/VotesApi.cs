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
    public static class VotesApi
    {
        [FunctionName("GetVotes")]
        public static async Task<IActionResult> GetVotes(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "votes")] HttpRequest req,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetVotes processed a request.");

            try
            {
                int? id = null;
                int? votedBy = null;
                int? votedFor = null;

                // Read optional query string parameters
                if (req.Query.ContainsKey("id") && int.TryParse(req.Query["id"], out int parsedId))
                {
                    id = parsedId;
                }

                if (req.Query.ContainsKey("votedBy") && int.TryParse(req.Query["votedBy"], out int parsedVotedBy))
                {
                    votedBy = parsedVotedBy;
                }

                if (req.Query.ContainsKey("votedFor") && int.TryParse(req.Query["votedFor"], out int parsedVotedFor))
                {
                    votedFor = parsedVotedFor;
                }

                DataTable dt = new DataTable();
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_ReadVote", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (id.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@ID", id.Value);
                    }

                    if (votedBy.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@VotedBy", votedBy.Value);
                    }

                    if (votedFor.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@VotedFor", votedFor.Value);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                var data = ModelConvertor.ConvertDataTable<Vote>(dt);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }
        [FunctionName("CreateVote")]
        public static async Task<IActionResult> CreateVote(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "votes")] HttpRequest req,
        ILogger log)
        {
            log.LogInformation("CreateVote HTTP trigger function processed a request.");
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var vote = JsonConvert.DeserializeObject<Vote>(requestBody);

                // Call the stored procedure to create the vote
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQLConnString")))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("sp_CreateVote", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@VotedDate", DateTime.UtcNow);
                        command.Parameters.AddWithValue("@IsUpVote", vote.IsUpVote);
                        command.Parameters.AddWithValue("@VotedBy", vote.VotedBy);
                        command.Parameters.AddWithValue("@VotedFor", vote.VotedFor);
                        var resultCodeParam = new SqlParameter("@ResultCode", SqlDbType.Int);
                        resultCodeParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultCodeParam);

                        await command.ExecuteNonQueryAsync();

                        int resultCode = (int)resultCodeParam.Value;

                        if (resultCode == 1)
                        {
                     //       return new OkResult();
                        }
                        else if (resultCode == 0)
                        {
                       //     return new BadRequestObjectResult("User has already voted for this user");
                        }
                        else
                        {
                         //   return new BadRequestObjectResult("Failed to create vote.");
                        }
                        return new ObjectResult(resultCode);
                    }
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }

    }
}
