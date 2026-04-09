using Azure.Storage.Blobs;

namespace Assignment01_EventSignup.Services
{
    public class BlobService : IBlobService
    {
        private readonly IConfiguration _configuration;

        public BlobService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string?> UploadFileAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var connectionString = _configuration["AzureBlobStorage:ConnectionString"];
            var containerName = _configuration["AzureBlobStorage:ContainerName"];

            var blobContainerClient = new BlobContainerClient(connectionString, containerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobClient.Uri.ToString();
        }
    }
}