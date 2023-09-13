using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Models;
using CupsellCloneAPI.Database.Repositories.Interfaces;

namespace CupsellCloneAPI.Core.Services;

public class OfferService
{
    private readonly IOfferRepository _offerRepository;
    public OfferService(IOfferRepository offerRepository)
    {
        _offerRepository = offerRepository;
    }

    public async Task<IEnumerable<Offer>> GetOffers(string? searchPhrase, int pageNumber, int pageSize,
        FilterOptionEnum filterOption, SortDirectionEnum sortDirection)
    {
        return await _offerRepository.GetFiltered(searchPhrase, pageNumber, pageSize, filterOption, sortDirection);
    }
}