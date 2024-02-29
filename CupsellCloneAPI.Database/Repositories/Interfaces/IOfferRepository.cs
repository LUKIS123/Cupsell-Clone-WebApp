using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Models;

namespace CupsellCloneAPI.Database.Repositories.Interfaces;

public interface IOfferRepository
{
    Task<Offer?> GetById(Guid id);

    Task<(IEnumerable<Offer> Offers, int Count)> GetFiltered(string? searchPhrase, int pageNumber, int pageSize,
        FilterOptionEnum sortBy, SortDirectionEnum sortDirection);

    Task<Guid> Create(Guid productId, Guid graphicId, Guid sellerId, decimal price, bool isAvailable,
        string? description);

    Task Delete(Guid id);

    Task Update(Guid id, Guid productId, Guid graphicId, Guid sellerId, decimal price, bool isAvailable,
        string? description);
}