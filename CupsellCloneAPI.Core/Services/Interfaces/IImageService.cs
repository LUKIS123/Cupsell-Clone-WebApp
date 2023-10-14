using CupsellCloneAPI.Database.BlobContainer.Models;

namespace CupsellCloneAPI.Core.Services.Interfaces
{
    public interface IImageService
    {
        Task<Dictionary<Guid, IEnumerable<string>>> GetOffersImagesUris(IEnumerable<Guid> offersId);
        Task<IEnumerable<string>> GetOfferImageUris(Guid offerId);
        Task<BlobObject> GetOfferImage(Guid offerName, string imageName);
        Task<BlobObject> GetGraphicImage(Guid graphicId, string imageName);
        Task<string> UploadOfferImage();
    }
}