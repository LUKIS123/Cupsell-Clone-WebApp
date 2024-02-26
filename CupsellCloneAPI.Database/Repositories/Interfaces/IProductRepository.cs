using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Models;

namespace CupsellCloneAPI.Database.Repositories.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetById(Guid productId);

    Task<IEnumerable<Product>> GetFiltered(string? searchPhrase, int pageNumber, int pageSize, FilterOptionEnum sortBy,
        SortDirectionEnum sortDirection, Guid sellerId);

    Task<Guid> Create(string name, string? description, int typeId, Guid sellerId);
    Task Delete(Guid id);
    Task Update(Guid id, string name, string? description, int typeId);
    Task<IEnumerable<ProductType>> GetProductTypes();
}