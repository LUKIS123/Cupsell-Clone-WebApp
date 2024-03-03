using Microsoft.AspNetCore.Mvc;
using CupsellCloneAPI.Core.Services.Interfaces;

namespace CupsellCloneAPI.Controllers;

[ApiController]
[Route("api/cupsellclone/assets")]
public class AssetsController : ControllerBase
{
    private readonly IAssetsService _imageService;

    public AssetsController(IAssetsService imageService)
    {
        _imageService = imageService;
    }

    [HttpGet("offers/{offerId:guid}/{imageName}")]
    public async Task<ActionResult> GetOfferImage([FromRoute] Guid offerId, [FromRoute] string imageName)
    {
        var result = await _imageService.GetOfferImage(offerId, imageName);
        return File(result.FileStream, result.ContentType);
    }

    [HttpGet("graphics/{graphicId:guid}/{imageName}")]
    public async Task<ActionResult> GetGraphicImage([FromRoute] Guid graphicId, [FromRoute] string imageName)
    {
        var result = await _imageService.GetGraphicImage(graphicId, imageName);
        return File(result.FileStream, result.ContentType);
    }

    [HttpGet("productTypes/{productId:guid}/{imageName}")]
    public async Task<ActionResult> GetProductTypeImage([FromRoute] Guid productId, [FromRoute] string imageName)
    {
        var result = await _imageService.GetProductImage(productId, imageName);
        return File(result.FileStream, result.ContentType);
    }
}