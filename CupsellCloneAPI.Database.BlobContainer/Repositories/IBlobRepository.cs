using CupsellCloneAPI.Database.BlobContainer.Models;
using Microsoft.AspNetCore.Http;

namespace CupsellCloneAPI.Database.BlobContainer.Repositories
{
    public interface IBlobRepository
    {
        Task<string> UploadBlobFile(IFormFile blob, string path);
        Task<BlobObject> DownloadBlobFile(string uri);
        Task DeleteBlob(string uri);
        Task<IEnumerable<string>> ListBlobs(string uri);
        Task<IEnumerable<string>> ListBlobs(string path, Guid id);
        Task<Dictionary<Guid, IEnumerable<string>>> ListBlobsByGuids(string path, IEnumerable<Guid> guids);
    }
}