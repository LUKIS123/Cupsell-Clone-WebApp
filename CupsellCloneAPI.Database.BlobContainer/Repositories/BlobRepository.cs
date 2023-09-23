using Azure.Storage.Blobs.Models;
using CupsellCloneAPI.Database.BlobContainer.Exceptions;
using CupsellCloneAPI.Database.BlobContainer.Factory;
using CupsellCloneAPI.Database.BlobContainer.Models;
using Microsoft.AspNetCore.Http;

namespace CupsellCloneAPI.Database.BlobContainer.Repositories;

public class BlobRepository : IBlobRepository
{
    private readonly IBlobServiceClientFactory _clientFactory;
    public static readonly List<string> ImageExtensions = new() { ".JPG", ".JPEG", ".PNG" };

    public BlobRepository(IBlobServiceClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<string> UploadBlobFile(IFormFile blob, string path)
    {
        var blobClient = _clientFactory.GetDefaultBlobContainerClient().GetBlobClient($"{path}/{blob.FileName}");

        if (await blobClient.ExistsAsync())
        {
            throw new BlobFileAlreadyExistsException($"File of given name [{blob.FileName}] already exists!");
        }

        await using (var data = blob.OpenReadStream())
        {
            await blobClient.UploadAsync(data);
        }

        return blobClient.Uri.AbsoluteUri;
    }

    // public async Task<BlobObject> DownloadBlobFile(string uri)
    // {
    //     var blobClient = _clientFactory.GetDefaultBlobContainerClient().GetBlobClient(uri);
    //     var result = new BlobObject();
    //     if (await blobClient.ExistsAsync())
    //     {
    //         var data = await blobClient.OpenReadAsync();
    //         var content = await blobClient.DownloadContentAsync();
    //
    //         result.FileStream = data;
    //         result.ContentType = content.Value.Details.ContentType;
    //     }
    //
    //     return result;
    // }

    public async Task<BlobObject> DownloadBlobFile(string uri)
    {
        var blobClient = _clientFactory.GetDefaultBlobContainerClient().GetBlobClient(uri);
        if (!await blobClient.ExistsAsync())
        {
            throw new BlobFileNotFoundException($"File [{uri}] does not exist!");
        }

        BlobDownloadResult content = await blobClient.DownloadContentAsync();
        var downloadedData = content.Content.ToStream();
        var fileName = uri.Split("/").Last();

        string contentType;
        if (ImageExtensions.Contains(Path.GetExtension(fileName.ToUpperInvariant())))
        {
            var extension = Path.GetExtension(fileName);
            contentType = "image/" + extension.Remove(0, 1);
        }
        else
        {
            contentType = content.Details.ContentType;
        }

        return new BlobObject()
        {
            FileStream = downloadedData,
            ContentType = contentType
        };
    }

    public async Task DeleteBlob(string uri)
    {
        var blobClient = _clientFactory.GetDefaultBlobContainerClient().GetBlobClient(uri);
        await blobClient.DeleteIfExistsAsync();
    }

    public async Task<IEnumerable<string>> ListBlobs(string uri)
    {
        var lst = new List<string>();
        await foreach (var blobItem in _clientFactory.GetDefaultBlobContainerClient().GetBlobsAsync(prefix: uri))
        {
            lst.Add(blobItem.Name);
        }

        return lst;
    }

    public async Task<IEnumerable<string>> ListBlobs(string path, Guid id)
    {
        var uri = $"{path}/{id}";
        var lst = new List<string>();
        await foreach (var blobItem in _clientFactory.GetDefaultBlobContainerClient().GetBlobsAsync(prefix: uri))
        {
            lst.Add(blobItem.Name);
        }

        return lst;
    }

    public async Task<Dictionary<Guid, IEnumerable<string>>> ListBlobsByGuids(string path, IEnumerable<Guid> guids)
    {
        var guidUrisDictionary = new Dictionary<Guid, IEnumerable<string>>();
        foreach (var guid in guids)
        {
            var uri = $"{path}/{guid}";
            var lst = new List<string>();
            await foreach (var blobItem in _clientFactory.GetDefaultBlobContainerClient().GetBlobsAsync(prefix: uri))
            {
                lst.Add(blobItem.Name);
            }

            guidUrisDictionary.Add(guid, lst);
        }

        return guidUrisDictionary;
    }
}