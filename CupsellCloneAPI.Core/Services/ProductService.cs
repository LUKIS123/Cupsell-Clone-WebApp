using AutoMapper;
using CupsellCloneAPI.Core.Exceptions;
using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos.Product;
using CupsellCloneAPI.Core.Services.Interfaces;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace CupsellCloneAPI.Core.Services
{
    internal class ProductService : IProductService
    {
        private readonly ILogger<IProductService> _logger;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly IAssetsService _assetsService;

        public ProductService(ILogger<IProductService> logger, IMapper mapper, IProductRepository productRepository,
            IAssetsService assetsService)
        {
            _logger = logger;
            _mapper = mapper;
            _productRepository = productRepository;
            _assetsService = assetsService;
        }

        public async Task<PageResult<ProductDto>> GetProducts(SearchQuery searchQuery)
        {
            var (enumerable, count) = await _productRepository.GetFiltered(
                searchQuery.SearchPhrase, searchQuery.PageNumber, searchQuery.PageSize, searchQuery.SortBy,
                searchQuery.SortDirection);
            var products = enumerable
                .ToList();
            var productImagesUris = await _assetsService.GetProductsImagesUris(
                products.Select(x => x.Id));

            var productDtoList = products.Select(product =>
            {
                var productDto = _mapper.Map<ProductDto>(product);

                if (productImagesUris.TryGetValue(productDto.Id, out var imageUri))
                {
                    productDto.ImageUri = imageUri;
                }

                return productDto;
            }).ToList();

            return new PageResult<ProductDto>
            {
                Items = productDtoList,
                TotalItemsCount = count,
                PageNumber = searchQuery.PageNumber,
                PageSize = searchQuery.PageSize
            };
        }

        public async Task<ProductDto> GetProductById(Guid id)
        {
            var productTask = _productRepository.GetById(id);
            var productImageTask = _assetsService.GetProductImageUri(id);
            await Task.WhenAll(productTask, productImageTask);

            var productDto = _mapper.Map<ProductDto>(productTask.Result);
            productDto.ImageUri = productImageTask.Result;

            return productDto;
        }

        public async Task<Guid> CreateProduct(CreateProductDto newProduct)
        {
            var productTypes = await _productRepository.GetProductTypes();

            var productTypeMatch = productTypes.FirstOrDefault(x =>
                x.Name.Equals(newProduct.ProductTypeName, StringComparison.CurrentCultureIgnoreCase));
            if (productTypeMatch is null)
            {
                throw new NotFoundException($"Product type: {newProduct.ProductTypeName} not found");
            }

            return await _productRepository.Create(newProduct.Name, newProduct.Description, productTypeMatch.Id);
        }

        public async Task UpdateProduct(Guid id, UpdateProductDto updatedProduct)
        {
            var productTypesTask = _productRepository.GetProductTypes();
            var productTask = _productRepository.GetById(id);
            await Task.WhenAll(productTypesTask, productTask);

            if (productTask.Result is null)
            {
                throw new NotFoundException($"Product with id:{id} not found");
            }

            var productTypeMatch = productTypesTask.Result.FirstOrDefault(x =>
                x.Name.Equals(updatedProduct.ProductTypeName, StringComparison.CurrentCultureIgnoreCase));
            if (productTypeMatch is null)
            {
                throw new NotFoundException($"Product type:{updatedProduct.ProductTypeName?.ToUpper()} not found");
            }

            _logger.LogInformation("Product with id:{ProductId} UPDATE action invoked...", id);

            await _productRepository.Update(id, updatedProduct.Name ?? productTask.Result.Name,
                updatedProduct.Description ?? productTask.Result.Description, productTypeMatch.Id);
        }

        public async Task DeleteProduct(Guid id)
        {
            _logger.LogInformation("Product with id:{ProductId} DELETE action invoked...", id);
            await _productRepository.Delete(id);
        }
    }
}