using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos.Offer;
using CupsellCloneAPI.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CupsellCloneAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/cupsellclone/offers")]
public class OfferController : ControllerBase
{
    private readonly IOfferService _offerService;
    private readonly IAssetsService _imageService;

    public OfferController(IOfferService offerService, IAssetsService imageService)
    {
        _offerService = offerService;
        _imageService = imageService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<PageResult<OfferDto>>> Get([FromQuery] SearchQuery searchQuery)
    {
        var offerDtosPageResult = await _offerService.GetOffers(searchQuery);
        return Ok(offerDtosPageResult);
    }

    [AllowAnonymous]
    [HttpGet("{offerId:guid}")]
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

    [HttpPost("upload/{offerId:guid}")]
    [Authorize(Roles = "Seller,Administrator")]
    public async Task<ActionResult> UploadOfferImage([FromRoute] Guid offerId, [FromForm] IFormFile blobFile)
    {
        await _imageService.UploadOfferImage(offerId, blobFile);
        return Ok();
    }

    [HttpPut("update/{offerId:guid}")]
    [Authorize(Roles = "Seller,Administrator")]
    public async Task<ActionResult> Update([FromRoute] Guid offerId, [FromBody] UpdateOfferDto dto)
    {
        await _offerService.Update(offerId, dto);
        return Ok();
    }

    [HttpDelete("delete/{offerId:guid}")]
    [Authorize(Roles = "Seller,Administrator")]
    public async Task<ActionResult> Delete([FromRoute] Guid offerId)
    {
        await _offerService.Delete(offerId);
        return Ok();
    }
}