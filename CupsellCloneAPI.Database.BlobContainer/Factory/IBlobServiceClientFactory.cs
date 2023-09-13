using Azure.Storage.Blobs;

namespace CupsellCloneAPI.Database.BlobContainer.Factory;

public interface IBlobServiceClientFactory
{
    public BlobServiceClient GetBlobServiceClient();
    public BlobContainerClient GetDefaultBlobContainerClient();
    public BlobContainerClient GetBlobContainerClient(string container);
}