using CupsellCloneAPI.Core.Services.Interfaces;
using CupsellCloneAPI.Database.BlobContainer.Models;
using CupsellCloneAPI.Database.BlobContainer.Repositories;

namespace CupsellCloneAPI.Core.Services;

public class ImageService : IImageService
{
    private readonly IBlobRepository _blobRepository;

    public static readonly string OfferCatalog = "offers";
    public static readonly string GraphicCatalog = "graphics";
    public static readonly string ImageControllerRoute = "cupsellclone/assets";

    public ImageService(IBlobRepository blobRepository)
    {
        _blobRepository = blobRepository;
    }

    public string MapOfferImageUrl(string path, string sourceUrl)
    {
        // TODO wywalic to i jednak dodac to metody ponizej mapowanie...
        var split = path.Split('/');
        return $"{sourceUrl}{ImageControllerRoute}/{split[1]}/{split[2]}";
    }

    public async Task<Dictionary<Guid, IEnumerable<string>>> GetOffersImagesUris(IEnumerable<Guid> offersId)
    {
        return await _blobRepository.ListBlobsByGuids(OfferCatalog, offersId);
    }

    public async Task<BlobObject> GetOfferImage(Guid offerId, string imageName)
    {
        var path = $"{GraphicCatalog}/{offerId}/{imageName}";
        return await _blobRepository.DownloadBlobFile(path);
    }
}