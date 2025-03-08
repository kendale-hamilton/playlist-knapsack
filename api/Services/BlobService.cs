using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Services.BlobService
{
    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly IConfiguration _configuration;
        public BlobService(IConfiguration configuration) 
        {
            _configuration = configuration;
            string connectionString = _configuration.GetSection("AzureStorage:ConnectionString").Value;
            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient("custom-playlists");
        }
        public async Task<bool> UploadFile(string blobName, Stream fileStream)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(fileStream, overwrite: true);
            return true;
        }
        public async Task<T> DownloadFile<T>(string blobName)
        {
            Console.WriteLine($"Downloading {blobName}");
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            var reader = new StreamReader(download.Content);
            string json = await reader.ReadToEndAsync();
            return JsonSerializer.Deserialize<T>(json);
        }
        public async Task<bool> DeleteFile(string blobName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
            return true;
        }
    }
}