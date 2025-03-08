
namespace Services.BlobService
{
    public interface IBlobService
    {
        Task<bool> UploadFile(string blobName, Stream fileStream);
        Task<T> DownloadFile<T>(string blobName);
        Task<bool> DeleteFile(string blobName);
    }
}