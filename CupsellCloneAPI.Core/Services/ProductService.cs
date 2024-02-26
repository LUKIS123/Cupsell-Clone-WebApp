using AutoMapper;
using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos.Product;
using CupsellCloneAPI.Core.Services.Interfaces;
using CupsellCloneAPI.Core.Utils.Accessors;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace CupsellCloneAPI.Core.Services
{
    internal class ProductService : IProductService
    {
        private readonly ILogger<IProductService> _logger;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly IAssetsService _assetsService;
        private readonly IUserAccessor _userAccessor;
        private readonly IAuthorizationService _authorizationService;

        public ProductService(ILogger<IProductService> logger, IMapper mapper, IProductRepository productRepository,
            IAssetsService assetsService, IUserAccessor userAccessor, IAuthorizationService authorizationService)
        {
            _logger = logger;
            _mapper = mapper;
            _productRepository = productRepository;
            _assetsService = assetsService;
            _userAccessor = userAccessor;
            _authorizationService = authorizationService;
        }

        public async Task<PageResult<ProductDto>> GetProducts(SearchQuery searchQuery)
        {
            var userId = _userAccessor.UserId;

            var products = (await _productRepository.GetFiltered(
                searchQuery.SearchPhrase, searchQuery.PageNumber, searchQuery.PageSize, searchQuery.SortBy,
                searchQuery.SortDirection, userId.Value
            )).ToList();

            throw new NotImplementedException();
        }

        public Task<ProductDto> GetProductById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CreateProduct(CreateProductDto newProduct)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProduct(Guid id, CreateProductDto updatedProduct)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProduct(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}