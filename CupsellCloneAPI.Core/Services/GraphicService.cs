using AutoMapper;
using CupsellCloneAPI.Authentication.Authorization;
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

        public async Task<GraphicDto> GetById(Guid id)
        {
            var graphicTask = _graphicRepository.GetById(id);
            var graphicImageTask = _assetsService.GetGraphicImageUri(id);
            await Task.WhenAll(graphicTask, graphicImageTask);

            var graphicDto = _mapper.Map<GraphicDto>(graphicTask.Result);
            graphicDto.ImageUri = graphicImageTask.Result;
            return graphicDto;
        }

        public async Task<Guid> Create(CreateGraphicDto newGraphic)
        {
            var userId = _userAccessor.UserId;
            if (userId is null)
            {
                throw new ForbidException("Could not access user Name Identifier");
            }

            return await _graphicRepository.Create(newGraphic.Name, userId.Value, newGraphic.Description);
        }

        public async Task Update(Guid id, UpdateGraphicDto updatedGraphic)
        {
            var userClaims = _userAccessor.UserClaimsPrincipal;
            if (userClaims is null)
            {
                throw new ForbidException("Could not verify user claims");
            }

            var graphic = await _graphicRepository.GetById(id);
            if (graphic is null)
            {
                throw new NotFoundException($"Graphic with id: {id} not found");
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(
                userClaims,
                graphic,
                new ResourceOperationRequirement(ResourceOperation.UPDATE));

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("User is not authorized to update this resource");
            }

            _logger.LogInformation("Graphic with id:{GraphicId} UPDATE action invoked...", id);

            await _graphicRepository.Update(id, updatedGraphic.Name ?? graphic.Name,
                graphic.Description ?? graphic.Description);
        }

        public async Task Delete(Guid id)
        {
            var userClaims = _userAccessor.UserClaimsPrincipal;
            if (userClaims is null)
            {
                throw new ForbidException("Could not verify user claims");
            }

            var graphic = await _graphicRepository.GetById(id);
            if (graphic is null)
            {
                throw new NotFoundException($"Graphic with id: {id} not found");
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(
                userClaims,
                graphic,
                new ResourceOperationRequirement(ResourceOperation.DELETE));

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("User is not authorized to delete this resource");
            }

            _logger.LogInformation("Graphic with id:{GraphicId} DELETE action invoked...", id);
            await _graphicRepository.Delete(id);
        }
    }
}