using AutoMapper;
using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos;
using CupsellCloneAPI.Core.Services.Interfaces;
using CupsellCloneAPI.Database.Repositories.Interfaces;

namespace CupsellCloneAPI.Core.Services;

public class OfferService : IOfferService
{
    private readonly IMapper _mapper;
    private readonly IOfferRepository _offerRepository;
    private readonly IAvailableItemsRepository _availableItemsRepository;
    private readonly IImageService _imageService;

    public OfferService(
        IMapper mapper,
        IOfferRepository offerRepository,
        IAvailableItemsRepository availableItemsRepository,
        IImageService imageService)
    {
        _mapper = mapper;
        _offerRepository = offerRepository;
        _availableItemsRepository = availableItemsRepository;
        _imageService = imageService;
    }

    public async Task<PageResult<OfferDto>> GetOffers(SearchQuery searchQuery, string sourceUrl)
    {
        var offers = (await _offerRepository.GetFiltered(
            searchQuery.SearchPhrase, searchQuery.PageNumber, searchQuery.PageSize, searchQuery.SortBy,
            searchQuery.SortDirection
        )).ToList();

        var availableItems =
            await _availableItemsRepository.GetAvailableItemsByOffersIds(offers.Select(x => x.Id));

        var offerImagesUris = await _imageService.GetOffersImagesUris(offers.Select(x => x.Id));

        var offerDtoList = offers
            .Select(offer =>
            {
                var offerDto = _mapper.Map<OfferDto>(offer);

                if (availableItems.TryGetValue(offerDto.Id, out var offerItems))
                {
                    offerDto.SizeQuantityDictionary = offerItems
                        .ToDictionary(x => x.Size.Name, x => x.Quantity);
                }

                if (offerImagesUris.TryGetValue(offerDto.Id, out var offerImageUris))
                {
                    var imageUrisList = offerImageUris.ToList();
                    var urlList = imageUrisList.Select(x => _imageService.MapOfferImageUrl(x, sourceUrl));
                    offerDto.ImageUrls = urlList;
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
}