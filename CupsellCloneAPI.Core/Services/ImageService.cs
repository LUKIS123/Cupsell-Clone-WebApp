using CupsellCloneAPI.Core.Services.Interfaces;
using CupsellCloneAPI.Database.BlobContainer.Models;
using CupsellCloneAPI.Database.BlobContainer.Repositories;

namespace CupsellCloneAPI.Core.Services;

public class ImageService : IImageService
{
    private readonly IBlobRepository _blobRepository;

    public static readonly string OfferCatalog = "offers";
    public static readonly string GraphicCatalog = "graphics";

    public ImageService(IBlobRepository blobRepository)
    {
        _blobRepository = blobRepository;
    }

    public async Task<Dictionary<Guid, IEnumerable<string>>> GetOffersImagesUris(IEnumerable<Guid> offersId)
    {
        return await _blobRepository.ListBlobsByGuids(OfferCatalog, offersId);
    }

    public async Task<IEnumerable<string>> GetOfferImageUris(Guid offerId)
    {
        return await _blobRepository.ListBlobs(OfferCatalog, offerId);
    }

    public async Task<BlobObject> GetOfferImage(Guid offerId, string imageName)
    {
        var path = $"{OfferCatalog}/{offerId}/{imageName}";
        return await _blobRepository.DownloadBlobFile(path);
    }

    public async Task<BlobObject> GetGraphicImage(Guid graphicId, string imageName)
    {
        var path = $"{GraphicCatalog}/{graphicId}/{imageName}";
        return await _blobRepository.DownloadBlobFile(path);
    }
}