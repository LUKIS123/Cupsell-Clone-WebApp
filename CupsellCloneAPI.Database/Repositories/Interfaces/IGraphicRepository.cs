using CupsellCloneAPI.Database.Entities.Product;

namespace CupsellCloneAPI.Database.Repositories.Interfaces;

public interface IGraphicRepository
{
    Task<Graphic?> GetById(Guid id);
    Task<IEnumerable<Graphic>> GetByUserId(Guid sellerId);
    Task<Guid> Create(string name, Guid sellerId, string? description);
    Task Delete(Guid id);
    Task Update(Guid id, string newName, string? description);
}