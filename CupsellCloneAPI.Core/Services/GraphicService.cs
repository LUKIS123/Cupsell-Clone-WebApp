using AutoMapper;
using CupsellCloneAPI.Core.Exceptions;
using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos.Graphic;
using CupsellCloneAPI.Core.Services.Interfaces;
using CupsellCloneAPI.Core.Utils.Accessors;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace CupsellCloneAPI.Core.Services
{
    internal class GraphicService : IGraphicService
    {
        private readonly ILogger<IGraphicService> _logger;
        private readonly IMapper _mapper;
        private readonly IGraphicRepository _graphicRepository;
        private readonly IAssetsService _assetsService;
        private readonly IUserAccessor _userAccessor;
        private readonly IAuthorizationService _authorizationService;

        public GraphicService(ILogger<IGraphicService> logger, IMapper mapper, IGraphicRepository graphicRepository,
            IAssetsService assetsService, IUserAccessor userAccessor, IAuthorizationService authorizationService)
        {
            _logger = logger;
            _mapper = mapper;
            _graphicRepository = graphicRepository;
            _assetsService = assetsService;
            _userAccessor = userAccessor;
            _authorizationService = authorizationService;
        }

        public async Task<PageResult<GraphicDto>> GetByUser(SearchQuery searchQuery)
        {
            var userId = _userAccessor.UserId;
            if (userId is null)
            {
                throw new ForbidException("Could not access user Name Identifier");
            }

            var (enumerable, count) = await _graphicRepository.GetFilteredByUser(userId.Value, searchQuery.SearchPhrase,
                searchQuery.PageNumber, searchQuery.PageSize, searchQuery.SortBy, searchQuery.SortDirection);
            var graphics = enumerable
                .ToList();
            var graphicImagesUris = await _assetsService.GetGraphicsImagesUris(graphics.Select(x => x.Id));

            var graphicDtoList = graphics.Select(graphic =>
            {
                var graphicDto = _mapper.Map<GraphicDto>(graphic);

                if (graphicImagesUris.TryGetValue(graphicDto.Id, out var imageUri))
                {
                    graphicDto.ImageUri = imageUri;
                }

                return graphicDto;
            }).ToList();

            return new PageResult<GraphicDto>
            {
                Items = graphicDtoList,
                TotalItemsCount = count,
                PageNumber = searchQuery.PageNumber,
                PageSize = searchQuery.PageSize
            };
        }

        public Task<GraphicDto> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> Create(CreateGraphicDto newGraphic)
        {
            throw new NotImplementedException();
        }

        public Task Update(Guid id, string graphicName)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}