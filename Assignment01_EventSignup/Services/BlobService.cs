using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Assignment01_EventSignup.Services
{
    public class BlobService : IBlobService
    {
        private readonly IConfiguration _configuration;

        public BlobService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            // ✅ Read from Azure App Settings
            var connectionString = _configuration["BlobConnectionString"];
            var containerName = _configuration["BlobContainer"];

            // ✅ Create Blob client
            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);

            // ✅ Ensure container exists (no public access)
            await containerClient.CreateIfNotExistsAsync();

            // ✅ Generate unique file name
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // ✅ Upload file
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                });
            }

            // ✅ Return URL
            return blobClient.Uri.ToString();
        }
    }
}