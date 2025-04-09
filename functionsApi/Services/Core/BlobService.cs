using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace Services.BlobService
{
    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _containerClient;
        public BlobService() 
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_BLOB_CONNECTION_STRING");
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