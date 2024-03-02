using AutoMapper;
using CupsellCloneAPI.Authentication.Authorization;
using CupsellCloneAPI.Core.Exceptions;
using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos.Offer;
using CupsellCloneAPI.Core.Services.Interfaces;
using CupsellCloneAPI.Core.Utils.Accessors;
using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace CupsellCloneAPI.Core.Services;

internal class OfferService : IOfferService
{
    private readonly ILogger<IOfferService> _logger;
    private readonly IMapper _mapper;
    private readonly IOfferRepository _offerRepository;
    private readonly IAvailableItemsRepository _availableItemsRepository;
    private readonly IAssetsService _assetsService;
    private readonly IUserAccessor _userAccessor;
    private readonly IAuthorizationService _authorizationService;

    public OfferService(
        ILogger<IOfferService> logger,
        IMapper mapper,
        IOfferRepository offerRepository,
        IAvailableItemsRepository availableItemsRepository,
        IAssetsService assetsService, IUserAccessor userAccessor,
        IAuthorizationService authorizationService)
    {
        _logger = logger;
        _mapper = mapper;
        _offerRepository = offerRepository;
        _availableItemsRepository = availableItemsRepository;
        _assetsService = assetsService;
        _userAccessor = userAccessor;
        _authorizationService = authorizationService;
    }

    public async Task<PageResult<OfferDto>> GetOffers(SearchQuery searchQuery)
    {
        var (enumerable, count) = await _offerRepository.GetFiltered(
            searchQuery.SearchPhrase, searchQuery.PageNumber, searchQuery.PageSize, searchQuery.SortBy,
            searchQuery.SortDirection
        );
        var offers = enumerable
            .ToList();

        var availableOffersItemsTask = _availableItemsRepository.GetAvailableItemsByOffersIds(offers
            .Select(x => x.Id));
        var offersImagesUrisTask = _assetsService.GetOffersImagesUris(offers.Select(x => x.Id));
        var graphicImagesUrisTask = _assetsService.GetGraphicsImagesUris(offers.Select(x => x.GraphicId));
        await Task.WhenAll(availableOffersItemsTask, offersImagesUrisTask, graphicImagesUrisTask);

        var offerDtoList = offers
            .Select(offer =>
            {
                var offerDto = _mapper.Map<OfferDto>(offer);

                if (availableOffersItemsTask.Result.TryGetValue(offerDto.Id, out var offerItems))
                {
                    offerDto.SizeQuantityDictionary = offerItems
                        .ToDictionary(x => x.SizeId,
                            x => new SizeQuantityDto
                            {
                                Id = x.Size.Id,
                                Name = x.Size.Name,
                                Quantity = x.Quantity
                            });
                }

                if (offersImagesUrisTask.Result.TryGetValue(offerDto.Id, out var offerImageUris))
                {
                    offerDto.ImageUrls = offerImageUris;
                }

                if (offerDto.Graphic is not null &&
                    graphicImagesUrisTask.Result.TryGetValue(offerDto.Graphic.Id, out var graphicImageUris))
                {
                    offerDto.Graphic.BlobUrl = graphicImageUris;
                }

                return offerDto;
            }).ToList();

        var results = new PageResult<OfferDto>
        {
            Items = offerDtoList,
            TotalItemsCount = count,
            PageSize = searchQuery.PageSize,
            PageNumber = searchQuery.PageNumber
        };

        return results;
    }

    public async Task<OfferDto> GetOfferById(Guid id)
    {
        var offerTask = _offerRepository.GetById(id);
        var availableItemsTask = _availableItemsRepository.GetAvailableItemsByOfferId(id);
        var offerImageUrisTask = _assetsService.GetOfferImageUris(id);
        await Task.WhenAll(offerTask, availableItemsTask, offerImageUrisTask);

        var offerDto = _mapper.Map<OfferDto>(offerTask.Result);
        offerDto.SizeQuantityDictionary = availableItemsTask.Result
            .ToDictionary(x => x.SizeId,
                x => new SizeQuantityDto
                {
                    Id = x.Size.Id,
                    Name = x.Size.Name,
                    Quantity = x.Quantity
                });
        offerDto.ImageUrls = offerImageUrisTask.Result;

        if (offerDto.Graphic is not null)
        {
            var graphicImagesUris = await _assetsService.GetGraphicImageUri(offerDto.Graphic.Id);
            offerDto.Graphic.BlobUrl = graphicImagesUris;
        }

        return offerDto;
    }

    public async Task<Guid> Create(CreateOfferDto dto)
    {
        var userId = _userAccessor.UserId;
        if (userId is null)
        {
            throw new ForbidException("Could not access user Name Identifier");
        }

        var createOfferTask = _offerRepository.Create(dto.ProductId, dto.GraphicsId, userId.Value, dto.Price, true,
            dto.Description);
        var getSizesTask = _availableItemsRepository.GetSizes();
        await Task.WhenAll(createOfferTask, getSizesTask);

        var sizeQuantityDictionary = dto.SizeIdQuantityDictionary.ToDictionary(keyValuePair =>
        {
            return new Size
            {
                Id = keyValuePair.Key,
                Name = getSizesTask.Result.FirstOrDefault(x => x.Id == keyValuePair.Key)?.Name!
            };
        }, keyValuePair => keyValuePair.Value);

        var offerId = createOfferTask.Result;
        await _availableItemsRepository.CreateItems(sizeQuantityDictionary, offerId);
        return offerId;
    }

    public async Task Update(Guid id, UpdateOfferDto dto)
    {
        var offer = await _offerRepository.GetById(id);
        if (offer is null)
        {
            throw new NotFoundException($"Offer with id:{id} not found");
        }

        var userClaims = _userAccessor.UserClaimsPrincipal;
        if (userClaims is null)
        {
            throw new ForbidException("Could not verify user claims");
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(
            userClaims,
            offer,
            new ResourceOperationRequirement(ResourceOperation.UPDATE));

        if (!authorizationResult.Succeeded)
        {
            throw new ForbidException("You cannot modify this resource");
        }

        _logger.LogInformation("Offer with id:{OfferId} UPDATE action invoked...", id);

        await _offerRepository.Update(id, (dto.ProductId ?? offer.ProductId),
            (dto.GraphicId ?? offer.GraphicId), offer.SellerId, (dto.Price ?? offer.Price),
            (dto.IsAvailable ?? offer.IsAvailable), (dto.Description ?? offer.Description));
    }

    public async Task Delete(Guid id)
    {
        var offer = _offerRepository.GetById(id);
        if (offer is null)
        {
            throw new NotFoundException($"Offer with id:{id} not found");
        }

        var userClaims = _userAccessor.UserClaimsPrincipal;
        if (userClaims is null)
        {
            throw new ForbidException("Could not verify user claims");
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(
            userClaims,
            offer,
            new ResourceOperationRequirement(ResourceOperation.DELETE));

        if (!authorizationResult.Succeeded)
        {
            throw new ForbidException("You cannot modify this resource");
        }

        _logger.LogInformation("Offer with id:{OfferId} DELETE action invoked...", id);

        await _offerRepository.Delete(id);
    }
}