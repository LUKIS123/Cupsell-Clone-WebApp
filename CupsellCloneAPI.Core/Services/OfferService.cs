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

public class OfferService : IOfferService
{
    private readonly ILogger<IOfferService> _logger;
    private readonly IMapper _mapper;
    private readonly IOfferRepository _offerRepository;
    private readonly IAvailableItemsRepository _availableItemsRepository;
    private readonly IImageService _imageService;
    private readonly IUserAccessor _userAccessor;
    private readonly IAuthorizationService _authorizationService;

    public OfferService(
        ILogger<IOfferService> logger,
        IMapper mapper,
        IOfferRepository offerRepository,
        IAvailableItemsRepository availableItemsRepository,
        IImageService imageService, IUserAccessor userAccessor,
        IAuthorizationService authorizationService)
    {
        _logger = logger;
        _mapper = mapper;
        _offerRepository = offerRepository;
        _availableItemsRepository = availableItemsRepository;
        _imageService = imageService;
        _userAccessor = userAccessor;
        _authorizationService = authorizationService;
    }

    public async Task<PageResult<OfferDto>> GetOffers(SearchQuery searchQuery)
    {
        var offers = (await _offerRepository.GetFiltered(
            searchQuery.SearchPhrase, searchQuery.PageNumber, searchQuery.PageSize, searchQuery.SortBy,
            searchQuery.SortDirection
        )).ToList();

        var availableOffersItems =
            await _availableItemsRepository.GetAvailableItemsByOffersIds(offers.Select(x => x.Id));

        var offersImagesUris = await _imageService.GetOffersImagesUris(offers.Select(x => x.Id));

        var offerDtoList = offers
            .Select(offer =>
            {
                var offerDto = _mapper.Map<OfferDto>(offer);

                if (availableOffersItems.TryGetValue(offerDto.Id, out var offerItems))
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

                if (offersImagesUris.TryGetValue(offerDto.Id, out var offerImageUris))
                {
                    offerDto.ImageUrls = offerImageUris;
                }

                return offerDto;
            }).ToList();

        var results = new PageResult<OfferDto>
        {
            Items = offerDtoList,
            TotalItemsCount = offerDtoList.Count,
            PageSize = searchQuery.PageSize,
            PageNumber = searchQuery.PageNumber
        };
        return results;
    }

    public async Task<OfferDto> GetOfferById(Guid id)
    {
        var offerTask = _offerRepository.GetById(id);
        var availableItemsTask = _availableItemsRepository.GetAvailableItemsByOfferId(id);
        var offerImageUrisTask = _imageService.GetOfferImageUris(id);
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
        return offerDto;
    }

    public async Task<Guid> Create(CreateOfferDto dto)
    {
        var userId = _userAccessor.UserId;
        if (userId is null)
        {
            throw new ForbidException("Could not access user Name Identifier");
        }

        var offerId = await _offerRepository.Create(dto.ProductId, dto.GraphicsId, userId.Value, dto.Price, true);
        var sizes = await _availableItemsRepository.GetSizes();

        var sizeQuantityDictionary = dto.SizeIdQuantityDictionary.ToDictionary(keyValuePair =>
        {
            return new Size
            {
                Id = keyValuePair.Key,
                Name = sizes.FirstOrDefault(x => x.Id == keyValuePair.Key)?.Name!
            };
        }, keyValuePair => keyValuePair.Value);

        await _availableItemsRepository.CreateItems(sizeQuantityDictionary, offerId);
        return offerId;
    }

    public async Task Update(Guid id, UpdateOfferDto dto)
    {
        // todo sprawdzic
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
            (dto.IsAvailable ?? offer.IsAvailable));
    }

    public async Task Delete(Guid id)
    {
        // todo sprawdzic 
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