using CupsellCloneAPI.Database.BlobContainer.Models;
using Microsoft.AspNetCore.Http;

namespace CupsellCloneAPI.Core.Services.Interfaces
{
    public interface IImageService
    {
        Task<Dictionary<Guid, IEnumerable<string>>> GetOffersImagesUris(IEnumerable<Guid> offersId);
        Task<Dictionary<Guid, string>> GetGraphicsImagesUris(IEnumerable<Guid> graphicId);
        Task<Dictionary<int, IEnumerable<string>>> GetProductTypeImagesUris(IEnumerable<int> productId);
        Task<IEnumerable<string>> GetOfferImageUris(Guid offerId);
        Task<string?> GetGraphicImageUri(Guid graphicId);
        Task<BlobObject> GetOfferImage(Guid offerName, string imageName);
        Task<BlobObject> GetGraphicImage(Guid graphicId, string imageName);
        Task<BlobObject> GetProductTypeImage(int productTypId, string imageName);
        Task<string> UploadOfferImage(Guid offerId, IFormFile blobFile);
    }
}