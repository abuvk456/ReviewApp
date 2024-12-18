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
    public static class NotificationsApi
    {
        [FunctionName("GetNotificationsByUserID")]
        public static IActionResult GetNotificationsByUserID(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "notifications/user/{userID}")] HttpRequest req,
            int userID,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetNotificationsByUserID processed a request.");

            try
            {
                DateTime? startDate = null;
                DateTime? endDate = null;
                bool? status = null;

                // Read optional query string parameters
                if (req.Query.ContainsKey("startDate") && DateTime.TryParse(req.Query["startDate"], out DateTime parsedStartDate))
                {
                    startDate = parsedStartDate;
                }

                if (req.Query.ContainsKey("endDate") && DateTime.TryParse(req.Query["endDate"], out DateTime parsedEndDate))
                {
                    endDate = parsedEndDate;
                }

                if (req.Query.ContainsKey("status") && bool.TryParse(req.Query["status"], out bool parsedStatus))
                {
                    status = parsedStatus;
                }

                DataTable dt = new DataTable();
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("GetNotificationByUserID", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserID", userID);

                    if (startDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                    }

                    if (endDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@EndDate", endDate.Value);
                    }

                    if (status.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@Status", status.Value);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                var data = ModelConvertor.ConvertDataTable<Notification>(dt);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }
        [FunctionName("DeleteNotification")]
        public static IActionResult DeleteNotification(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "notifications/{id}")] HttpRequest req,
            int id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function DeleteNotification processed a request.");

            try
            {
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DeleteNotification", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);
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

