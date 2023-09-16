using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos;
using CupsellCloneAPI.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CupsellCloneAPI.Controllers;

[ApiController]
[Route("cupsellclone/offers")]
public class OfferController : ControllerBase
{
    private readonly IOfferService _offerService;

    public OfferController(IOfferService offerService)
    {
        _offerService = offerService;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<OfferDto>>> GetOffers([FromQuery] SearchQuery searchQuery)
    {
        var sourceUrl = HttpContext.Request.GetDisplayUrl();
        var offerDtosPageResult = await _offerService.GetOffers(
            searchQuery,
            sourceUrl[..sourceUrl.IndexOf("cupsellclone", StringComparison.Ordinal)]
        );
        return Ok(offerDtosPageResult);
    }
}