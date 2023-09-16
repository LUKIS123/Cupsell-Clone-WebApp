using CupsellCloneAPI.Database.BlobContainer.Models;

namespace CupsellCloneAPI.Core.Services.Interfaces
{
    public interface IImageService
    {
        public string MapOfferImageUrl(string path, string sourceUrl);
        Task<Dictionary<Guid, IEnumerable<string>>> GetOffersImagesUris(IEnumerable<Guid> offersId);
        Task<BlobObject> GetOfferImage(Guid offerName, string imageName);
    }
}