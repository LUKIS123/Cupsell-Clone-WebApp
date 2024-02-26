using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Repositories.Interfaces;

namespace CupsellCloneAPI.Database.Repositories
{
    public class ProductTypeRepository : IProductTypeRepository
    {
        public Task<IEnumerable<ProductType>> GetAll(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<ProductType?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> Create(string name)
        {
            throw new NotImplementedException();
        }

        public Task Update(int id, string name)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}