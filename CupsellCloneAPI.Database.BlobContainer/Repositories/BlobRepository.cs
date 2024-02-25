using Azure.Storage.Blobs.Models;
using CupsellCloneAPI.Database.BlobContainer.Exceptions;
using CupsellCloneAPI.Database.BlobContainer.Factory;

namespace CupsellCloneAPI.Database.BlobContainer.Repositories;

public class BlobRepository : IBlobRepository
{
    private readonly IBlobServiceClientFactory _clientFactory;

    public BlobRepository(IBlobServiceClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<string> UploadBlobFile(byte[] bytes, string fileName, string path)
    {
        var blobClient = _clientFactory.GetDefaultBlobContainerClient()
            .GetBlobClient($"{path}/{fileName}");

        if (await blobClient.ExistsAsync())
        {
            throw new BlobFileAlreadyExistsException($"File of given name [{fileName}] already exists!");
        }

        using var stream = new MemoryStream(bytes);
        await blobClient.UploadAsync(stream);

        return blobClient.Uri.AbsoluteUri;
    }

    public async Task<Tuple<Stream, string>> DownloadBlobFile(string uri)
    {
        var blobClient = _clientFactory.GetDefaultBlobContainerClient()
            .GetBlobClient(uri);
        if (!await blobClient.ExistsAsync())
        {
            throw new BlobFileNotFoundException($"File [{uri}] does not exist!");
        }

        BlobDownloadResult content = await blobClient.DownloadContentAsync();
        var downloadedData = content.Content.ToStream();

        return Tuple.Create(downloadedData, content.Details.ContentType);
    }

    public async Task DeleteBlob(string uri)
    {
        var blobClient = _clientFactory.GetDefaultBlobContainerClient()
            .GetBlobClient(uri);
        await blobClient.DeleteIfExistsAsync();
    }

    public async Task<IEnumerable<string>> ListBlobs(string uri)
    {
        return await _clientFactory.GetDefaultBlobContainerClient()
            .GetBlobsAsync(prefix: uri)
            .Select(blobItem => blobItem.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> ListBlobs(string path, Guid id)
    {
        var uri = $"{path}/{id}";

        return await _clientFactory.GetDefaultBlobContainerClient()
            .GetBlobsAsync(prefix: uri)
            .Select(blobItem => blobItem.Name)
            .ToListAsync();
    }

    public async Task<string?> ListBlob(string path, Guid id)
    {
        var uri = $"{path}/{id}";
        var items = await _clientFactory.GetDefaultBlobContainerClient()
            .GetBlobsAsync(prefix: uri)
            .FirstOrDefaultAsync();
        return items?.Name;
    }

    public async Task<Dictionary<Guid, IEnumerable<string>>> ListBlobsByGuids(string path, IEnumerable<Guid> guids)
    {
        var guidUrisDictionary = new Dictionary<Guid, IEnumerable<string>>();
        foreach (var guid in guids)
        {
            var uri = $"{path}/{guid}";
            var lst = await _clientFactory.GetDefaultBlobContainerClient()
                .GetBlobsAsync(prefix: uri)
                .Select(blobItem => blobItem.Name)
                .ToListAsync();

            guidUrisDictionary.Add(guid, lst);
        }

        return guidUrisDictionary;
    }

    public async Task<Dictionary<Guid, string>> ListBlobByGuids(string path, IEnumerable<Guid> guids)
    {
        var guidUrisDictionary = new Dictionary<Guid, string>();

        var blob = _clientFactory.GetDefaultBlobContainerClient();
        foreach (var guid in guids)
        {
            var uri = $"{path}/{guid}";
            var item = await blob.GetBlobsAsync(prefix: uri).FirstOrDefaultAsync();

            if (item is not null)
            {
                guidUrisDictionary.Add(guid, item.Name);
            }
        }

        return guidUrisDictionary;
    }

    public async Task<Dictionary<int, IEnumerable<string>>> ListBlobsByIds(string path, IEnumerable<int> ids)
    {
        var idUrisDictionary = new Dictionary<int, IEnumerable<string>>();
        foreach (var id in ids)
        {
            var uri = $"{path}/{id}";
            var lst = await _clientFactory.GetDefaultBlobContainerClient()
                .GetBlobsAsync(prefix: uri)
                .Select(blobItem => blobItem.Name)
                .ToListAsync();

            idUrisDictionary.Add(id, lst);
        }

        return idUrisDictionary;
    }
}