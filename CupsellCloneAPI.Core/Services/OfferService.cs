using AutoMapper;
using CupsellCloneAPI.Core.Exceptions;
using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos.Offer;
using CupsellCloneAPI.Core.Services.Interfaces;
using CupsellCloneAPI.Core.Utils.Accessors;
using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Repositories.Interfaces;

namespace CupsellCloneAPI.Core.Services;

public class OfferService : IOfferService
{
    private readonly IMapper _mapper;
    private readonly IOfferRepository _offerRepository;
    private readonly IAvailableItemsRepository _availableItemsRepository;
    private readonly IImageService _imageService;
    private readonly IUserAccessor _userAccessor;

    public OfferService(
        IMapper mapper,
        IOfferRepository offerRepository,
        IAvailableItemsRepository availableItemsRepository,
        IImageService imageService, IUserAccessor userAccessor)
    {
        _mapper = mapper;
        _offerRepository = offerRepository;
        _availableItemsRepository = availableItemsRepository;
        _imageService = imageService;
        _userAccessor = userAccessor;
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
                        .ToDictionary(x => new SizeDto
                        {
                            Id = x.SizeId,
                            Name = x.Size.Name
                        }, x => x.Quantity);
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
        offerDto.SizeQuantityDictionary = availableItemsTask.Result.ToDictionary(x => new SizeDto
        {
            Id = x.SizeId,
            Name = x.Size.Name
        }, x => x.Quantity);
        offerDto.ImageUrls = offerImageUrisTask.Result;
        return offerDto;
    }

    public async Task<Guid> Create(CreateOfferDto dto)
    {
        var userId = _userAccessor.UserId;
        if (userId == null)
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
        throw new NotImplementedException();
        //todo
        //await _offerRepository.Update(id);
    }
}