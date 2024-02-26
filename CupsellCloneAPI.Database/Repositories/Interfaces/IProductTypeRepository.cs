using CupsellCloneAPI.Database.Entities.Product;

namespace CupsellCloneAPI.Database.Repositories.Interfaces
{
    public interface IProductTypeRepository
    {
        Task<IEnumerable<ProductType>> GetAll(int pageNumber, int pageSize);
        Task<ProductType?> GetById(int id);
        Task<Guid> Create(string name);
        Task Update(int id, string name);
        Task Delete(int id);
    }
}