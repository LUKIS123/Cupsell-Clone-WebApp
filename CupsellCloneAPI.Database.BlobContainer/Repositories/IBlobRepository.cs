using CupsellCloneAPI.Database.BlobContainer.Models;
using Microsoft.AspNetCore.Http;

namespace CupsellCloneAPI.Database.BlobContainer.Repositories
{
    public interface IBlobRepository
    {
        Task<string> UploadBlobFile(IFormFile blob, string path);
        Task<BlobObject> DownloadBlobFile(string uri);
        Task<IEnumerable<BlobObject>> DownloadBlobFilesInDirectory(string prefix);
        Task DeleteBlob(string uri);
        Task<IEnumerable<string>> ListBlobs();
    }
}