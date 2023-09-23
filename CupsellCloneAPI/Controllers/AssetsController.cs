using Microsoft.AspNetCore.Mvc;
using CupsellCloneAPI.Core.Services.Interfaces;

namespace CupsellCloneAPI.Controllers;

[ApiController]
[Route("cupsellclone/assets")]
public class AssetsController : ControllerBase
{
    private readonly IImageService _imageService;

    public AssetsController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpGet("offers/{offerId}/{imageName}")]
    public async Task<ActionResult> GetOfferImage([FromRoute] Guid offerId, [FromRoute] string imageName)
    {
        var result = await _imageService.GetOfferImage(offerId, imageName);
        return File(result.FileStream, result.ContentType);
    }

    [HttpGet("graphics/{graphicId}/{imageName}")]
    public async Task<ActionResult> GetGraphicImage([FromRoute] Guid graphicId, [FromRoute] string imageName)
    {
        var result = await _imageService.GetGraphicImage(graphicId, imageName);
        return File(result.FileStream, result.ContentType);
    }
}