using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Models;

namespace CupsellCloneAPI.Database.Repositories.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetById(Guid productId);

    Task<(IEnumerable<Product> Products, int Count)> GetFiltered(string? searchPhrase, int pageNumber, int pageSize,
        FilterOptionEnum sortBy,
        SortDirectionEnum sortDirection);

    Task<Guid> Create(string name, string? description, int typeId);
    Task Delete(Guid id);
    Task Update(Guid id, string name, string? description, int typeId);
    Task<IEnumerable<ProductType>> GetProductTypes();
}