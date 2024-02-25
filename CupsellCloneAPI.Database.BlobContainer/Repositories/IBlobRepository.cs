namespace CupsellCloneAPI.Database.BlobContainer.Repositories
{
    public interface IBlobRepository
    {
        Task<string> UploadBlobFile(byte[] bytes, string fileName, string path);
        Task<Tuple<Stream, string>> DownloadBlobFile(string uri);
        Task DeleteBlob(string uri);
        Task<IEnumerable<string>> ListBlobs(string uri);
        Task<IEnumerable<string>> ListBlobs(string path, Guid id);
        Task<string?> ListBlob(string path, Guid id);
        Task<Dictionary<Guid, IEnumerable<string>>> ListBlobsByGuids(string path, IEnumerable<Guid> guids);
        Task<Dictionary<Guid, string>> ListBlobByGuids(string path, IEnumerable<Guid> guids);
        Task<Dictionary<int, IEnumerable<string>>> ListBlobsByIds(string path, IEnumerable<int> ids);
    }
}