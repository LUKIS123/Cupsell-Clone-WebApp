using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Models;

namespace CupsellCloneAPI.Database.Repositories.Interfaces;

public interface IGraphicRepository
{
    Task<Graphic?> GetById(Guid id);
    Task<(IEnumerable<Graphic> Graphics, int Count)> GetFiltered(Guid sellerId);
    Task<Guid> Create(string name, Guid sellerId, string? description);
    Task Delete(Guid id);
    Task Update(Guid id, string newName, string? description);

    Task<(IEnumerable<Graphic> Graphics, int Count)> GetFilteredByUser
    (Guid sellerId, string? searchQuerySearchPhrase, int searchQueryPageNumber, int searchQueryPageSize,
        FilterOptionEnum searchQuerySortBy, SortDirectionEnum searchQuerySortDirection);
}