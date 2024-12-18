using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ReviewAppAFAPP.Functions
{
    public static class UploadFile
    {
        [FunctionName("UploadFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                // Retrieve the file from the request
                var file = req.Form.Files["file"];

                // Validate if a file was received
                if (file == null || file.Length == 0)
                {
                    return new BadRequestObjectResult("No file was received.");
                }

                // Retrieve the connection string for Azure Blob Storage
                var storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

                // Create a CloudStorageAccount object
                var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

                // Create a CloudBlobClient object
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Create a container reference (assuming the container already exists)
                var container = blobClient.GetContainerReference("reviewapp");

                // Create the container if it doesn't exist
                await container.CreateIfNotExistsAsync();

                // Set the access level for the container (optional)
                await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                // Generate a unique name for the blob
                var fileName = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + Path.GetExtension(file.FileName);

                // Create a CloudBlockBlob object with the unique name
                var blob = container.GetBlockBlobReference(fileName);

                // Upload the file to Azure Blob Storage
                using (var stream = file.OpenReadStream())
                {
                    await blob.UploadFromStreamAsync(stream);
                }

                // Get the URL of the uploaded file
                var fileUrl = blob.Uri.ToString();

                // Return the URL as the response
                return new OkObjectResult(fileUrl);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "An error occurred while processing the file upload.");
                return new StatusCodeResult(500);
            }
        }
    }
}
