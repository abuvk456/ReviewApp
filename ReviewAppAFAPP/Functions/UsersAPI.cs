using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;
using ReviewAppAFAPP.Extensions;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using CommonModel;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using ReviewAppAFAPP.Functions;


namespace ReviewAppAFAPP
{
    public static class UsersAPI
    {
        [FunctionName(nameof(GetUsers))]
        public static async Task<IActionResult> GetUsers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Users")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetUsers processed a request.");

            try
            {
                DataTable dt = new DataTable();
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    string spName = "sp_GetUsers";
                    SqlCommand cmd = new SqlCommand(spName, con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Set the parameters for the stored procedure
                    string userIdParam = req.Query["userId"];
                    string usernameParam = req.Query["username"];
                    string emailParam = req.Query["email"];
                    string nameParam = req.Query["name"];
                    cmd.Parameters.AddWithValue("@UserId", !string.IsNullOrEmpty(userIdParam) ? int.Parse(userIdParam) : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Username", !string.IsNullOrEmpty(usernameParam) ? usernameParam : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", !string.IsNullOrEmpty(emailParam) ? emailParam : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", !string.IsNullOrEmpty(nameParam) ? nameParam : (object)DBNull.Value);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                var data = ModelConvertor.ConvertDataTable<User>(dt);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }
        [FunctionName(nameof(GetUsersSameWatchList))]
        public static async Task<IActionResult> GetUsersSameWatchList(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Userswithsamewatchlist")] HttpRequest req,
    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetUsersSameWatchList processed a request.");

            try
            {
                DataTable dt = new DataTable();
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    string spName = "sp_GetUsersSameWatchList";
                    SqlCommand cmd = new SqlCommand(spName, con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Set the parameters for the stored procedure
                    string userIdParam = req.Query["userId"];
                    cmd.Parameters.AddWithValue("@UserId", !string.IsNullOrEmpty(userIdParam) ? int.Parse(userIdParam) : (object)DBNull.Value);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                var data = ModelConvertor.ConvertDataTable<User>(dt);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }


        [FunctionName(nameof(UserSignIn))]
        public static async Task<IActionResult> UserSignIn(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Users/SignIn")] HttpRequest req,
    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function UserSignIn processed a request.");
            DataTable dt = new DataTable();
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var user = JsonConvert.DeserializeObject<UserCredentials>(requestBody);

                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_SignInUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Email", user.Email.ToLower());
                    cmd.Parameters.AddWithValue("@Password", user.Password);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);


                    var data = ModelConvertor.ConvertDataTable<User>(dt);

                    var userdb = data?.FirstOrDefault();
                    if (userdb != null)
                    {
                        {
                            // Create a JWT token
                            var tokenHandler = new JwtSecurityTokenHandler();
                            //        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JwtSecret"));
                            var key = Encoding.ASCII.GetBytes("HelloIDontHaveLongKey");
                            var tokenDescriptor = new SecurityTokenDescriptor
                            {
                                Subject = new ClaimsIdentity(new Claim[]
                                {
                        new Claim(ClaimTypes.NameIdentifier, userdb.UserId.ToString())
                                }),
                                Expires = DateTime.UtcNow.AddDays(7),
                                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                            };
                            var token = tokenHandler.CreateToken(tokenDescriptor);
                            // Insert the session into the Sessions table
                            using (SqlCommand insertCmd = new SqlCommand("sp_InsertSession", con))
                            {
                                insertCmd.CommandType = CommandType.StoredProcedure;
                                insertCmd.Parameters.AddWithValue("@UserId", userdb.UserId);
                                //insertCmd.Parameters.AddWithValue("@Token", tokenHandler.WriteToken(token));
                                insertCmd.Parameters.AddWithValue("@Token", "NOTOKEN");
                                insertCmd.Parameters.AddWithValue("@Expiration", DateTime.UtcNow.AddDays(7));
                                insertCmd.Parameters.AddWithValue("@CreatedDate", DateTime.UtcNow);
                                insertCmd.Parameters.AddWithValue("@IsActive", true);
                                await insertCmd.ExecuteNonQueryAsync();
                            }
                            userdb.SessionInfo = new Session
                            {
                                UserId = userdb.UserId,
                                Token = "NOTOKEN",
                                // Token = tokenHandler.WriteToken(token),
                                ExpirationDate = DateTime.UtcNow.AddDays(7),
                                SessionId = 0,
                                IsActive = true,
                                CreatedDate = DateTime.UtcNow,
                            };
                            return new OkObjectResult(userdb);
                        }
                    }
                    else
                    {
                        return new BadRequestObjectResult("Invalid credentials");
                    }
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }


        [FunctionName(nameof(GetUserById))]
        public static async Task<IActionResult> GetUserById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Users/{Id}")] HttpRequest req,
        string Id,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetUserById processed a request.");

            try
            {
                DataTable dt = new DataTable();
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_GetUsers", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", Id);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                var data = ModelConvertor.ConvertDataTable<User>(dt);

                return new OkObjectResult(data?.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }


        [FunctionName(nameof(AddUser))]
        public static async Task<IActionResult> AddUser(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Users")] HttpRequest req,
    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function AddUser processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var user = JsonConvert.DeserializeObject<User>(requestBody);

                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                int cmdResult = 0;
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_InsertUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Country", user.Country);
                    cmd.Parameters.AddWithValue("@Language", user.Language);
                    cmd.Parameters.AddWithValue("@UpvoteCount", 0);
                    cmd.Parameters.AddWithValue("@DownvoteCount", 0);
                    cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                    cmd.Parameters.AddWithValue("@Age", user.Age);

                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        cmdResult = Convert.ToInt32(result);
                    }
                }

                if (cmdResult == 0)
                {
                    return new OkObjectResult(-1);
                }
                try
                {
                    Helper.sendEmail(user.Email, "Welcome to Review App", "Thank you for signing up for our app! We are thrilled to have you on board and want to extend a warm welcome to you.");
                }
                catch
                {

                }
                return new OkObjectResult(cmdResult);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }



        [FunctionName(nameof(UpdateUser))]
        public static async Task<IActionResult> UpdateUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Users")] HttpRequest req,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function UpdateUser processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var user = JsonConvert.DeserializeObject<User>(requestBody);

                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                var cmdResult = 0;
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_UpdateUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@userid", user.UserId);
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Country", user.Country);
                    cmd.Parameters.AddWithValue("@Language", user.Language);
                    cmd.Parameters.AddWithValue("@UpvoteCount", user.UpvoteCount);
                    cmd.Parameters.AddWithValue("@DownvoteCount", user.DownvoteCount);
                    cmd.Parameters.AddWithValue("@IsActive", user.IsActive);

                    cmdResult = cmd.ExecuteNonQuery();
                }

                return new OkObjectResult(cmdResult);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }


        [FunctionName(nameof(DeleteUser))]
        public static async Task<IActionResult> DeleteUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Users/{Id}")] HttpRequest req,
        string Id,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function DeleteUser processed a request.");

            try
            {
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                var cmdResult = 0;
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_DeleteUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmdResult = cmd.ExecuteNonQuery();
                }

                return new OkObjectResult(cmdResult);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }
        [FunctionName(nameof(GetUserStats))]
        public static async Task<IActionResult> GetUserStats(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "UserStats/{userId}")] HttpRequest req,
            int userId,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetUserStats processed a request.");

            try
            {
                DataTable dt = new DataTable();
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    string spName = "sp_GetUserStats";
                    SqlCommand cmd = new SqlCommand(spName, con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Set the parameters for the stored procedure
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                var data = ModelConvertor.ConvertDataTable<UserProfileInfo>(dt);
                return new OkObjectResult(data.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
        [FunctionName("CreateFollowing")]
        public static async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "followings/{FollowedBy}/{FollowedUserId}")] HttpRequest req,
      int FollowedBy, int FollowedUserId,
      ILogger log)
        {
            log.LogInformation("C# HTTP trigger function CreateFollowing processed a request.");

            try
            {
                int rowsAffected = 0;
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("CreateFollowing", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FollowedUserId", FollowedUserId);
                    cmd.Parameters.AddWithValue("@FollowedBy", FollowedBy);

                    SqlParameter rowsAffectedParam = new SqlParameter("@RowsAffected", SqlDbType.Int);
                    rowsAffectedParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(rowsAffectedParam);

                    cmd.ExecuteNonQuery();

                    rowsAffected = (int)cmd.Parameters["@RowsAffected"].Value;
                }

                return new OkObjectResult(rowsAffected);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error in CreateFollowingFunction");
                return new BadRequestResult();
            }
        }
        [FunctionName(nameof(GetFollowings))]
       public static async Task<IActionResult> GetFollowings(
       [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Followings/{FollowedBy}")] HttpRequest req, int FollowedBy,
       ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetUsers processed a request.");

            try
            {
                DataTable dt = new DataTable();
                string strConString = Environment.GetEnvironmentVariable("SQLConnString");
                using (SqlConnection con = new SqlConnection(strConString))
                {
                    con.Open();
                    string spName = "ReadFollowing";
                    SqlCommand cmd = new SqlCommand(spName, con);
                    cmd.CommandType = CommandType.StoredProcedure;



                    cmd.Parameters.AddWithValue("@FollowedBy", FollowedBy);


                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                var data = ModelConvertor.ConvertDataTable<User>(dt);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Data);
            }
        }
        [FunctionName("ChangePassword")]
        public static async Task<IActionResult> ChangePassword(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "changepassword")] HttpRequest req,
    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function ChangePassword processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                int userId = data?.UserId;
                string oldPassword = data?.OldPassword;
                string newPassword = data?.NewPassword;

                string connectionString = Environment.GetEnvironmentVariable("SQLConnString");
                int rowsAffected = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_ChangePassword", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@OldPassword", oldPassword);
                        command.Parameters.AddWithValue("@NewPassword", newPassword);

                        SqlParameter rowsAffectedParam = new SqlParameter("@RowsAffected", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(rowsAffectedParam);

                        await command.ExecuteNonQueryAsync();

                        rowsAffected = (int)rowsAffectedParam.Value;
                    }
                }

                return new OkObjectResult(rowsAffected);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error in ChangePassword function.");
                return new BadRequestResult();
            }
        }
        [FunctionName("ResetPassword")]
        public static async Task<IActionResult> ResetPassword(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "ResetPassword")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function ChangePassword processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                string email = data?.Email;
                string newPassword = data?.NewPassword;

                string connectionString = Environment.GetEnvironmentVariable("SQLConnString");
                string result = "";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_ResetPassword", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@NewPassword", newPassword);

                        //SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.NVarChar, 100)
                        //{
                        //    Direction = ParameterDirection.Output
                        //};
                        //command.Parameters.Add(resultParam);

                        await command.ExecuteNonQueryAsync();
                        result= "1";
                        //result = (string)resultParam.Value;
                    }
                }
                try
                {
                    Helper.sendEmail("abivk456@gmail.com", "Welcome to Review App", $"Please use this:  {newPassword} : to login ");
                }
                catch
                {

                }
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error in ChangePassword function.");
                return new BadRequestResult();
            }
        }


    }



}

