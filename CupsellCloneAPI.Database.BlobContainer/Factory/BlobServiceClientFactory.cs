using Azure.Storage.Blobs;

namespace CupsellCloneAPI.Database.BlobContainer.Factory;

public class BlobServiceClientFactory : IBlobServiceClientFactory
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _blobContainerClient;
    private readonly string _blobContainerName;

    public BlobServiceClientFactory(string connectionString, string container)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
        _blobContainerClient = _blobServiceClient.GetBlobContainerClient(container);
        _blobContainerName = container;
    }

    public BlobServiceClient GetBlobServiceClient()
    {
        return _blobServiceClient;
    }

    public BlobContainerClient GetDefaultBlobContainerClient()
    {
        return _blobContainerClient;
    }

    public BlobContainerClient GetBlobContainerClient(string uri)
    {
        return _blobServiceClient.GetBlobContainerClient($"{_blobContainerName}/{uri}");
    }
}