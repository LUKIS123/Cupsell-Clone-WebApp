using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos.Offer;

namespace CupsellCloneAPI.Core.Services.Interfaces
{
    public interface IOfferService
    {
        Task<PageResult<OfferDto>> GetOffers(SearchQuery searchQuery);
        Task<OfferDto> GetOfferById(Guid id);
        Task<Guid> Create(CreateOfferDto dto);
        Task Update(Guid id, UpdateOfferDto dto);
    }
}