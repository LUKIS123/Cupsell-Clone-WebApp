using CupsellCloneAPI.Core.Models.Dtos;
using CupsellCloneAPI.Core.Models;

namespace CupsellCloneAPI.Core.Services.Interfaces
{
    public interface IOfferService
    {
        Task<PageResult<OfferDto>> GetOffers(SearchQuery searchQuery);
        Task<OfferDto> GetOfferById(Guid id);
    }
}