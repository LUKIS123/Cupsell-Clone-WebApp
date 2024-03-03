using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Models;

namespace CupsellCloneAPI.Database.Repositories.Interfaces;

public interface IGraphicRepository
{
    Task<Graphic?> GetById(Guid id);

    Task<(IEnumerable<Graphic> Graphics, int Count)> GetFilteredByUser
    (Guid sellerId, string? searchPhrase, int pageNumber, int pageSize,
        FilterOptionEnum sortBy, SortDirectionEnum sortDirection);

    Task<Guid> Create(string name, Guid sellerId, string? description);
    Task Delete(Guid id);
    Task Update(Guid id, string newName, string? description);
}