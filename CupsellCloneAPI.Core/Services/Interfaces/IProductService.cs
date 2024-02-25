using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos.Product;

namespace CupsellCloneAPI.Core.Services.Interfaces
{
    public interface IProductService
    {
        Task<PageResult<ProductDto>> GetAllProducts();
        Task<ProductDto> GetProductById(Guid id);
        Task<Guid> CreateProduct(CreateProductDto newProduct);
        Task UpdateProduct(Guid id, CreateProductDto updatedProduct);
        Task DeleteProduct(Guid id);
    }
}