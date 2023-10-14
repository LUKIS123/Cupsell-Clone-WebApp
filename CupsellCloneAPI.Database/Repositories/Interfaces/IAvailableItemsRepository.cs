using CupsellCloneAPI.Database.Entities.Product;

namespace CupsellCloneAPI.Database.Repositories.Interfaces;

public interface IAvailableItemsRepository
{
    Task<AvailableItem?> GetById(Guid id);
    Task<Dictionary<Guid, IEnumerable<AvailableItem>>> GetAvailableItemsByOffersIds(IEnumerable<Guid> offerIds);
    Task<IEnumerable<AvailableItem>> GetAvailableItemsByOfferId(Guid offerId);
    Task<Dictionary<Size, Guid>> CreateItems(Dictionary<Size, int> sizeQuantityDictionary, Guid offerId);
    Task Delete(Guid id);
    Task Update(Guid id, int sizeId, int quantity);
    Task<IEnumerable<Size>> GetSizes();
    Task<Size?> GetSizeById(int sizeId);
}