using CupsellCloneAPI.Database.BlobContainer.Models;
using Microsoft.AspNetCore.Http;

namespace CupsellCloneAPI.Core.Services.Interfaces
{
    public interface IAssetsService
    {
        Task<Dictionary<Guid, IEnumerable<string>>> GetOffersImagesUris(IEnumerable<Guid> offersId);
        Task<Dictionary<Guid, string>> GetGraphicsImagesUris(IEnumerable<Guid> graphicId);
        Task<Dictionary<Guid, string>> GetProductsImagesUris(IEnumerable<Guid> productId);
        Task<IEnumerable<string>> GetOfferImageUris(Guid offerId);
        Task<string?> GetGraphicImageUri(Guid graphicId);
        Task<string?> GetProductImageUri(Guid productId);
        Task<BlobObject> GetOfferImage(Guid offerName, string imageName);
        Task<BlobObject> GetGraphicImage(Guid graphicId, string imageName);
        Task<BlobObject> GetProductImage(Guid productId, string imageName);
        Task<string> UploadOfferImage(Guid offerId, IFormFile blobFile);
        Task<string> UploadProductImage(Guid productId, IFormFile blobFile);
    }
}