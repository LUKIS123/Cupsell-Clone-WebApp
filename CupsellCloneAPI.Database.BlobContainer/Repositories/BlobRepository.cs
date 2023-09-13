using Azure.Storage.Blobs.Models;
using CupsellCloneAPI.Database.BlobContainer.Exceptions;
using CupsellCloneAPI.Database.BlobContainer.Factory;
using CupsellCloneAPI.Database.BlobContainer.Models;
using Microsoft.AspNetCore.Http;

namespace CupsellCloneAPI.Database.BlobContainer.Repositories;

public class BlobRepository : IBlobRepository
{
    private readonly IBlobServiceClientFactory _clientFactory;
    public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPEG", ".PNG" };

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
        var result = new BlobObject();
        if (await blobClient.ExistsAsync())
        {
            BlobDownloadResult content = await blobClient.DownloadContentAsync();
            var downloadedData = content.Content.ToStream();

            var fileName = new Uri(uri).Segments.LastOrDefault();
            if (ImageExtensions.Contains(Path.GetExtension(fileName.ToUpperInvariant())))
            {
                var extension = Path.GetExtension(fileName);
                result.ContentType = "image/" + extension.Remove(0, 1);
            }
            else
            {
                result.ContentType = content.Details.ContentType;
            }

            result.FileStream = downloadedData;
        }

        return result;
    }

    // public async Task<BlobObject> GetBlobFile(string url)
    // {
    //     var fileName = new Uri(url).Segments.LastOrDefault();
    //
    //     try
    //     {
    //         var blobClient = _clientFactory.GetDefaultBlobContainerClient().GetBlobClient(fileName);
    //         if (await blobClient.ExistsAsync())
    //         {
    //             BlobDownloadResult content = await blobClient.DownloadContentAsync();
    //             var downloadedData = content.Content.ToStream();
    //
    //             if (ImageExtensions.Contains(Path.GetExtension(fileName.ToUpperInvariant())))
    //             {
    //                 var extension = Path.GetExtension(fileName);
    //                 return new BlobObject
    //                     { FileStream = downloadedData, ContentType = "image/" + extension.Remove(0, 1) };
    //             }
    //             else
    //             {
    //                 return new BlobObject { FileStream = downloadedData, ContentType = content.Details.ContentType };
    //             }
    //         }
    //         else
    //         {
    //             return null;
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         throw;
    //     }
    // }

    // public async Task<IEnumerable<BlobObject>> DownloadBlobFilesInDirectory(string prefix)
    // {
    //     var blobs = _clientFactory.GetDefaultBlobContainerClient().GetBlobsAsync(prefix: prefix);
    //     var lst = new List<BlobObject>();
    //     await foreach (var blobItem in blobs)
    //     {
    //         var blobClient = _clientFactory.GetDefaultBlobContainerClient().GetBlobClient(blobItem.Name);
    //         var data = await blobClient.OpenReadAsync();
    //         var content = await blobClient.DownloadContentAsync();
    //
    //         lst.Add(new BlobObject() { FileStream = data, ContentType = content.Value.Details.ContentType });
    //     }
    //
    //     return lst;
    // }

    public async Task<IEnumerable<BlobObject>> DownloadBlobFilesInDirectory(string prefix)
    {
        var blobs = _clientFactory.GetDefaultBlobContainerClient().GetBlobsAsync(prefix: prefix);
        var lst = new List<BlobObject>();
        await foreach (var blobItem in blobs)
        {
            var blobClient = _clientFactory.GetDefaultBlobContainerClient().GetBlobClient(blobItem.Name);
            var result = new BlobObject();

            BlobDownloadResult content = await blobClient.DownloadContentAsync();
            var downloadedData = content.Content.ToStream();

            if (ImageExtensions.Contains(Path.GetExtension(blobItem.Name.ToUpperInvariant())))
            {
                var extension = Path.GetExtension(blobItem.Name);
                result.ContentType = "image/" + extension.Remove(0, 1);
            }
            else
            {
                result.ContentType = content.Details.ContentType;
            }

            result.FileStream = downloadedData;
            lst.Add(result);
        }

        return lst;
    }

    public async Task DeleteBlob(string uri)
    {
        var blobClient = _clientFactory.GetDefaultBlobContainerClient().GetBlobClient(uri);
        await blobClient.DeleteIfExistsAsync();
    }

    public async Task<IEnumerable<string>> ListBlobs()
    {
        var lst = new List<string>();
        await foreach (var blobItem in _clientFactory.GetDefaultBlobContainerClient().GetBlobsAsync())
        {
            lst.Add(blobItem.Name);
        }

        return lst;
    }
}