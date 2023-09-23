using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos;
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

    public OfferController(IOfferService offerService)
    {
        _offerService = offerService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<PageResult<OfferDto>>> GetOffers([FromQuery] SearchQuery searchQuery)
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
}