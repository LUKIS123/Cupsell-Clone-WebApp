using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos;
using CupsellCloneAPI.Core.Models.Dtos.Offer;
using CupsellCloneAPI.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CupsellCloneAPI.Controllers;

[ApiController]
[Authorize]
[Route("cupsellclone/offers")]
public class OfferController : ControllerBase
{
    private readonly IOfferService _offerService;
    private readonly IImageService _imageService;

    public OfferController(IOfferService offerService)
    {
        _offerService = offerService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<PageResult<OfferDto>>> Get([FromQuery] SearchQuery searchQuery)
    {
        var offerDtosPageResult = await _offerService.GetOffers(searchQuery);
        return Ok(offerDtosPageResult);
    }

    [AllowAnonymous]
    [HttpGet("{offerId}")]
    public async Task<ActionResult<OfferDto>> GetById([FromRoute] Guid offerId)
    {
        var offerDto = await _offerService.GetOfferById(offerId);
        return Ok(offerDto);
    }

    [HttpPost]
    [Authorize(Roles = "Seller,Administrator")]
    public async Task<ActionResult> Create([FromBody] CreateOfferDto dto)
    {
        var createdOfferId = await _offerService.Create(dto);
        return Created(Request.Scheme + "://" + Request.Host + Url.Action("GetById", "Offer"),
            new { offerId = createdOfferId });
    }

    [HttpPost("{offerId}")]
    [Authorize(Roles = "Seller,Administrator")]
    public async Task<ActionResult> UploadOfferImage()
    {
        await _imageService.UploadOfferImage();
        return Ok();
    }

    [HttpPut("{offerId}")]
    [Authorize(Roles = "Seller,Administrator")]
    public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UpdateOfferDto dto)
    {
        return Ok();
    }
}