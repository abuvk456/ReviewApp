using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAppAFAPP.Functions
{
    public class Helper
    {
        public static string sendEmail(string receiver, string subject, string body)
        {
            using (WebClient client = new WebClient())
            {
                NameValueCollection values = new NameValueCollection();
                values.Add("apikey", "BCE0F3A71352D78CAC4F1C5A692516C3F5894F6C417422552A2DB1ED7D331E21411E41E92DA8F36B786CF22C6CC60B09");
                values.Add("from", "asghir.baig@gmail.com");
                values.Add("fromName", "Reivew App");
                values.Add("to", receiver);
                values.Add("subject", subject);
                values.Add("bodyText", body);
                //values.Add("bodyHtml", $"<h1>body</h1>");
                //values.Add("isTransactional", true);

                string address = "https://api.elasticemail.com/v2/email/send";

                try
                {
                    byte[] apiResponse = client.UploadValues(address, values);
                    return Encoding.UTF8.GetString(apiResponse);

                }
                catch (Exception ex)
                {
                    return "Exception caught: " + ex.Message + "\n" + ex.StackTrace;
                }
            }
        }
        public static string GeneratePassword()
        {
            int passwordLength = 8; // Length of the password
            string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"; // Characters allowed in the password
            RandomNumberGenerator rng = RandomNumberGenerator.Create();

            byte[] randomBytes = new byte[passwordLength];
            rng.GetBytes(randomBytes);

            string password = new string(randomBytes
                .Select(x => allowedChars[x % allowedChars.Length])
                .ToArray());
            return password;
        }
    }
}
